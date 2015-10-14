

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using Mono.Xml;



namespace behaviac
{
    public struct property_t
    {
        public string name;
        public string value;

        public property_t(string n, string v)
        {
            name = n;
            value = v;
        }
    }

    public abstract class BehaviorNode
    {
        private string m_className;
        private int m_id;

        public EBTStatus m_status;
        protected List<BehaviorNode> m_attachments;
        public List<Property> m_pars;

        protected BehaviorNode m_parent;
        protected List<BehaviorNode> m_children;

        public CMethodBase m_enterAction;
        public CMethodBase m_exitAction;

        protected bool m_bHasEvents;

        protected BehaviorNode()
        {
        }

        ~BehaviorNode()
        {
            this.Clear();
        }

        public virtual void AddChild(BehaviorNode pChild)
        {
            pChild.m_parent = this;

            if (this.m_children == null)
            {
                this.m_children = new List<BehaviorNode>();
            }

            this.m_children.Add(pChild);
        }

        public void Attach(BehaviorNode pAttachment)
        {
            if (this.m_attachments == null)
            {
                this.m_attachments = new List<BehaviorNode>();
            }

            this.m_attachments.Add(pAttachment);
        }

        public void AddPar(string type, string name, string value, string eventParam)
        {
            Property pProperty = Property.Create(type, name, value, false, false);
            if (!string.IsNullOrEmpty(eventParam))
            {
                pProperty.SetRefName(eventParam);
            }

            if (this.m_pars == null)
            {
                this.m_pars = new List<Property>();
            }
            this.m_pars.Add(pProperty);
        }

        public void Clear()
        {
            if (this.m_attachments != null)
            {
                this.m_attachments.Clear();
                this.m_attachments = null;
            }

            if (this.m_children != null)
            {
                this.m_children.Clear();
                this.m_children = null;
            }
        }

        protected abstract BehaviorTask createTask();

        public BehaviorTask CreateAndInitTask()
        {
            BehaviorTask pTask = this.createTask();
            pTask.Init(this);

            return pTask;
        }

        protected static BehaviorNode Create(string className)
        {
            return Workspace.CreateBehaviorNode(className);
        }

        public virtual bool enteraction_impl(Agent pAgent) { return false; }

        public virtual bool exitaction_impl(Agent pAgent) { return false; }

        public int GetChildrenCount()
        {
            if (this.m_children != null)
            {
                return this.m_children.Count;
            }

            return 0;
        }

        public BehaviorNode GetChild(int index)
        {
            if (this.m_children != null && index < this.m_children.Count)
            {
                return this.m_children[index];
            }

            return null;
        }

        public int GetAttachmentsCount()
        {
            if (this.m_attachments != null)
            {
                return this.m_attachments.Count;
            }

            return 0;
        }

        public BehaviorNode GetAttachment(int index)
        {
            if (this.m_attachments != null && index < this.m_attachments.Count)
            {
                return this.m_attachments[index];
            }

            return null;
        }

        public void SetClassNameString(string className)
        {
            this.m_className = className;
        }

        public string GetClassNameString()
        {
            return this.m_className;
        }

        public int GetId()
        {
            return this.m_id;
        }

        public void SetId(int id)
        {
            this.m_id = id;
        }

        public bool HasEvents()
        {
            return this.m_bHasEvents;
        }

#if !BEHAVIAC_RELEASE
        private string m_agentType;

        public void SetAgentType(string agentType)
        {
            this.m_agentType = agentType.Replace("::", ".");
        }

        public string GetAgentType()
        {
            return this.m_agentType;
        }
#endif
        public virtual bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
#if !BEHAVIAC_RELEASE
            Debug.Check(!string.IsNullOrEmpty(this.m_agentType));

            return Agent.IsDerived(pAgent, this.m_agentType);
#else
            return true;
#endif//#if !BEHAVIAC_RELEASE
        }

        protected virtual void load(int version, string agentType, List<property_t> properties)
        {
            foreach (property_t p in properties)
            {
                if (p.name == "EnterAction")
                {
                    if (!string.IsNullOrEmpty(p.value))
                    {
                        this.m_enterAction = Action.LoadMethod(p.value);
                    }//if (p.value[0] != '\0')
                }
                else if (p.name == "ExitAction")
                {
                    if (!string.IsNullOrEmpty(p.value))
                    {
                        this.m_exitAction = Action.LoadMethod(p.value);
                    }//if (p.value[0] != '\0')
                }
            }

            //string nodeType = this.GetClassNameString().Replace(".", "::");
            //Workspace.HandleNodeLoaded(nodeType, properties);
        }

        protected static BehaviorNode load(string agentType, SecurityElement node)
        {
            Debug.Check(node.Tag == "node");

            int version = int.Parse(node.Attribute("version"));

            string pClassName = node.Attribute("class");
            BehaviorNode pNode = BehaviorNode.Create(pClassName);

            Debug.Check(pNode != null);
            if (pNode != null)
            {
                pNode.SetClassNameString(pClassName);
                string idStr = node.Attribute("id");
                pNode.SetId(Convert.ToInt32(idStr));

                pNode.load_properties_pars_attachments_children(true, version, agentType, node);
            }

            return pNode;
        }

		protected void load_properties_pars_attachments_children(bool bNode, int version, string agentType, SecurityElement node)
        {
#if !BEHAVIAC_RELEASE
            SetAgentType(agentType);
#endif//#ifdef _DEBUG

            bool bHasEvents = this.HasEvents();

            if (node.Children != null)
            {
                List<property_t> properties = new List<property_t>();

                foreach (SecurityElement c in node.Children)
                {
                    if (!load_property_pars(ref properties, c, version, agentType))
                    {
                        if (bNode)
                        {
                            if (c.Tag == "attachment")
                            {
                                string pAttachClassName = c.Attribute("class");
                                BehaviorNode pAttachment = BehaviorNode.Create(pAttachClassName);

                                Debug.Check(pAttachment != null);

                                if (pAttachment != null)
                                {
                                    pAttachment.SetClassNameString(pAttachClassName);
                                    string idStr = c.Attribute("id");
                                    pAttachment.SetId(Convert.ToInt32(idStr));

                                    pAttachment.load_properties_pars_attachments_children(false, version, agentType, c);

                                    this.Attach(pAttachment);

                                    bHasEvents |= (pAttachment is Event);
                                }
                            }
                            else if (c.Tag == "node")
                            {
                                BehaviorNode pChildNode = BehaviorNode.load(agentType, c);
                                bHasEvents |= pChildNode.m_bHasEvents;

                                this.AddChild(pChildNode);
                            }
                        }
                    }
                }

                if (properties.Count > 0)
                {
                    this.load(version, agentType, properties);
                }
            }

            this.m_bHasEvents |= bHasEvents;
        }

        private bool load_property_pars(ref List<property_t> properties, SecurityElement c, int version, string agentType)
        {
            if (c.Tag == "property")
            {
                if (c.Attributes.Count == 1)
                {
                    foreach (string propName in c.Attributes.Keys)
                    {
                        string propValue = (string)c.Attributes[propName];
                        property_t p = new property_t(propName, propValue);
                        properties.Add(p);
                        break;
                    }
                }

                return true;
            }
            else if (c.Tag == "pars")
            {
                if (c.Children != null)
                {
                    foreach (SecurityElement parNode in c.Children)
                    {
                        if (parNode.Tag == "par")
                        {
                            this.load_par(version, agentType, parNode);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        void load_par(int version, string agentType, SecurityElement node)
        {
            if (node.Tag != "par")
            {
                Debug.Check(false);
                return;
            }

            string name = node.Attribute("name");
            string type = node.Attribute("type").Replace("::", ".");
            string value = node.Attribute("value");
            string eventParam = node.Attribute("eventParam");

            this.AddPar(type, name, value, eventParam);
        }

        public void SetHasEvents(bool hasEvents)
        {
            this.m_bHasEvents = hasEvents;
        }

        //the bt export type is c# will generate update_impl.
        protected virtual EBTStatus update_impl(Agent pAgent, EBTStatus childStatus) { return EBTStatus.BT_FAILURE; }

    }

    // ============================================================================

    public abstract class DecoratorNode : BehaviorNode
    {
        public DecoratorNode()
        {
            m_bDecorateWhenChildEnds = false;
        }

        ~DecoratorNode()
        { }


        public bool m_bDecorateWhenChildEnds;
    }

    // ============================================================================

    public class BehaviorTree : BehaviorNode
    {
        public class Descriptor_t
        {
            public Property Descriptor;
            public Property Reference;

            public Descriptor_t()
            { }

            public Descriptor_t(Descriptor_t copy)
            {
                Descriptor = copy.Descriptor != null ? copy.Descriptor.clone() : null;
                Reference = copy.Reference != null ? copy.Reference.clone() : null;
            }

            ~Descriptor_t()
            {
                this.Descriptor = null;
                this.Reference = null;
            }
        };


        protected override BehaviorTask createTask()
        {
            BehaviorTreeTask pTask = new BehaviorTreeTask();
            return pTask;
        }

        //return the path relative to the workspace path
        protected string m_name;
        public string GetName()
        {
            return this.m_name;
        }

        protected string m_domains;

        public string GetDomains()
        {
            return this.m_domains;
        }

        public void SetDomains(string domains)
        {
            this.m_domains = domains;
        }

        List<Descriptor_t> m_descriptorRefs;

        public List<Descriptor_t> GetDescriptors()
        {
            return m_descriptorRefs;
        }

        public void SetDescriptors(string descriptors)
        {
            this.m_descriptorRefs = (List<Descriptor_t>)StringUtils.FromString(typeof(List<Descriptor_t>), descriptors, false);

            for (int i = 0; i < this.m_descriptorRefs.Count; ++i)
            {
                Descriptor_t d = this.m_descriptorRefs[i];
                d.Descriptor.SetDefaultValue(d.Reference);
            }
        }
        /**
        <?xml version="1.0" encoding="utf-8"?>
        <behavior agenttype="AgentTest">
        <!--EXPORTED BY TOOL, DON'T MODIFY IT!-.
        <!--Source File: ... -.
        <node class="DecoratorLoopTask">
        <property Count="10" />
        <node class="SelectorTask">
            ...
        </node>
        </node>
        </behavior>
        */
        public bool load_xml(byte[] pBuffer)
        {
            try
            {
                Debug.Check(pBuffer != null);
                string xml = System.Text.Encoding.UTF8.GetString(pBuffer);

                SecurityParser xmlDoc = new SecurityParser();
                xmlDoc.LoadXml(xml);

                SecurityElement behaviorNode = xmlDoc.ToXml();
                if (behaviorNode.Tag != "behavior" && (behaviorNode.Children == null || behaviorNode.Children.Count != 1))
                {
                    return false;
                }

                this.m_name = behaviorNode.Attribute("name");
                string agentType = behaviorNode.Attribute("agenttype");
                string versionStr = behaviorNode.Attribute("version");
                int version = int.Parse(versionStr);

                //this.SetClassNameString("BehaviorTree");
                //this.SetId(-1);

                this.load_properties_pars_attachments_children(true, version, agentType, behaviorNode);

                return true;
            }
            catch (Exception e)
            {
                Debug.Check(false, e.Message);
            }

            Debug.Check(false);
            return false;
        }
    }

}


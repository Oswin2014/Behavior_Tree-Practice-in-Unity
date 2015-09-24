
using UnityEngine;
using System.Diagnostics;
using System.IO;

namespace behavior
{
    
    public class FileManager
    {

        private static FileManager ms_instance = null;
        public FileManager()
        {
            Debug.Check(ms_instance == null);
            ms_instance = this;
        }

        public static FileManager Instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new FileManager();
                }

                return ms_instance;
            }
        }


        /// <summary>
        /// open the specified file, this function should be consistent with 
        /// Workspace.SetWorkspaceSettings's first param 'workspaceExportPath' and Workspace.Load's first param 'relativePath'
        /// as 'filePath' is the conbination of workspaceExportPath and relativePath
        /// 
        /// you may need to override this function if you gave a customized 'workspaceExportPath' or used a AssetBundle.
        /// </summary>
        /// <returns>The open.</returns>
        /// <param name="filePath">without extension</param>
        /// <param name="ext">'ext' coult be .xml or .bson</param>
        public virtual byte[] FileOpen(string filePath, string ext)
        {
            try
            {
#if !UNITY_WEBPLAYER && (UNITY_EDITOR || UNITY_STANDALONE_WIN)
                //if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    if (ext == ".bson")
                    {
                        ext += ".bytes";
                    }

					filePath += ext;
					byte[] pBuffer = File.ReadAllBytes(filePath);
					
					return pBuffer;
                }
#else
                {
                    if (ext == ".bson")
                    {
                        filePath += ext;
                    }

                    //skip 'Resources/'
                    int k0 = filePath.IndexOf("Resources");

                    if (k0 != -1)
                    {
                        k0 += 10;
                        string filePathInResources = filePath.Substring(k0);

                        TextAsset ta = Resources.Load(filePathInResources) as TextAsset;

                        if (ta == null)
                        {
                            string msg = string.Format("FileManager::FileOpen failed:'{0}' not loaded", filePath);
                            behaviac.Debug.LogWarning(msg);

                            return null;
                        }
                        else
                        {
                            byte[] pBuffer = ta.bytes;

                            return pBuffer;
                        }
                    }
                    else
                    {
                        string msg = string.Format("FileManager::FileOpen failed:'{0}' should be in /Resources", filePath);
                        behaviac.Debug.LogWarning(msg);
                    }
                }
#endif
            }
            catch
            {
                string msg = string.Format("FileManager::FileOpen exception:'{0}'", filePath);
                behavior.Debug.LogWarning(msg);
            }

            return null;
        }
    }

}

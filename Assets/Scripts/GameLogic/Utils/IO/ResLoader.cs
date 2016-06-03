
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace GameLogic.IO
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public class ResLoader
    {
        #region 加载文件
        //-------------------------------------------------------------------------
        #region 加载系统目录下的文件
        /// <summary>
        /// 加载安装目录下的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static String LoadStringFromSystem(String fileName)
        {
            string result;
            var fspath = String.Concat(Application.persistentDataPath, "/", fileName);
            if (File.Exists(fspath))
            {
                result = fspath.LoadFile();
            }
            else
            {
                result = LoadStringFromResource(fileName.TrimSuffix());
            }
            return result;
        }
        //-------------------------------------------------------------------------
        public static byte[] LoadByteFromSystem(String fileName)
        {
            if (File.Exists(fileName))
            {
                return File.ReadAllBytes(fileName);
            }
            else
            {
                return null;
            }
        }
        //-------------------------------------------------------------------------
        #endregion


        #region 加载Resource目录下的文件
        //-------------------------------------------------------------------------
        public static String LoadStringFromResource(String fileName)
        {
            var text = Resources.Load(fileName);
            if (text != null)
            {
                var result = text.ToString();
                Resources.UnloadAsset(text);
                return result;
            }
            else
            {
                return String.Empty;
            }
        }
        //-------------------------------------------------------------------------
        public static byte[] LoadByteFromResource(String fileName)
        {

            byte[] result = null;
            var binAsset = Resources.Load<TextAsset>(fileName);
            if (binAsset != null)
            {
                result = binAsset.bytes;
                Resources.UnloadAsset(binAsset);
            }
            return result;
        }
        //-------------------------------------------------------------------------
        public static Stream LoadStreamFromResource(String fileName)
        {
            Stream result = null;
            var binAsset = UnityEngine.Resources.Load<UnityEngine.Object>(fileName) as TextAsset;
            if (binAsset != null)
            {
                result = new MemoryStream(binAsset.bytes);
                //StreamWriter writer = new StreamWriter(result);
                //writer.Write(binAsset.ToString());
                //writer.Flush();
                Resources.UnloadAsset(binAsset);
            }
            return result;
        }
        //-------------------------------------------------------------------------
        #endregion
        //-------------------------------------------------------------------------
        /// <summary>
        /// 根据文件绝对路径加载文件
        /// </summary>
        /// <param name="filePath">绝对路径</param>
        /// <returns>文件内容</returns>
        public static string readTextFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    return sr.ReadToEnd().Trim();
                }
            }
            else
            {
                return String.Empty;
            }
        }

        public static string readTextFileFromResource(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return string.Empty;

            TextAsset text = Resources.Load(filePath) as TextAsset;
            return text.text;
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// 根据文件绝对路径加载文件
        /// </summary>
        /// <param name="filePath">绝对路径</param>
        /// <returns>文件内容</returns>
        public static byte[] LoadByteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            else
            {
                return null;
            }
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="filePath">写入文件的绝对路径</param>
        /// <param name="content">写入内容</param>
        public static void WriterFile(string filePath, string content)
        {
            string path = filePath.Replace("\\", "/");
            string dicPath = path.Substring(0, path.LastIndexOf('/'));
            if (!Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
            }           
            using (FileStream fs = File.OpenWrite(path))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(content);
                sw.Flush();
            }
        }
        //-------------------------------------------------------------------------
        #endregion
        //-------------------------------------------------------------------------
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        public static void WriterFile(string filePath, byte[] bytes)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("Disk path is null!!!");
                return;
            }

            string path = filePath.Replace("\\", "/");
            string dicPath = path.Substring(0, path.LastIndexOf('/'));

            try
            {
                if (!Directory.Exists(dicPath))      //创建文件夹
                {
                    Directory.CreateDirectory(dicPath);
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.WriteAllBytes(path, bytes);
            }
            catch (Exception e)
            {
                Debug.LogError("WriterFile Error:::" + e.Message);
            }

        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// 覆盖式写入文本文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public static void WriteCoverTextFile(string fileName, string content)
        {
            if(Application.isWebPlayer)
            {
                return;
            }

            //文件流信息
            StreamWriter sw;
            FileInfo t = new FileInfo(fileName);
            if (!t.Exists)
            {
                //如果此文件不存在则创建
                sw = t.CreateText();
            }
            else
            {
                sw = new StreamWriter(fileName, false);
            }
            //以行的形式写入信息 
            sw.WriteLine(content);
            //关闭流
            sw.Close();
            //销毁流
            sw.Dispose();
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// 读取StreamingAssets目录下本地文本文件,Android平台下要通过www类读取
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="onRead"></param>
		public static IEnumerator ReadStreamingFile(string filePath,Action<string> onRead)
        {
            string data = null;

            WWW www = new WWW(filePath);
            yield return www;

            if(!string.IsNullOrEmpty(www.error))
            {
                Debug.LogWarning("ReadStreamingFile error : " + www.error);
                yield break;
            }

            data = www.text.TrimEnd();
            www.Dispose();

            if (onRead != null)
                onRead(data);

            yield return null;
        }
        //-------------------------------------------------------------------------
    }
}

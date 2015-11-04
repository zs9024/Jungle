using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

/// <summary>
/// 创建资源的MD5值，并写到xml
/// </summary>
public class CreateMD5List
{
    public static void Execute(UnityEditor.BuildTarget target)
    {
        string platform = AssetBundleController.GetPlatformName(target);
        Execute(platform);

        AssetDatabase.Refresh();
    }

    public static void Execute(string platform)
    {
        Dictionary<string, string> dicFileMD5 = new Dictionary<string, string>();   //key:文件名 value:MD5值
        MD5CryptoServiceProvider md5Generator = new MD5CryptoServiceProvider();

        string dir = System.IO.Path.Combine(Application.dataPath, "AssetBundle/" + platform);

        List<string> pathList = new List<string>();
        pathList = GetAllFilePath(dir, pathList);

        foreach (string filePath in pathList)
        {
            if (filePath.Contains(".meta") || filePath.Contains("VersionMD5") || filePath.Contains(".xml"))
                continue;

            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] hash = md5Generator.ComputeHash(file);
            string strMD5 = System.BitConverter.ToString(hash);
            file.Close();

            string key = filePath.Substring(dir.Length + 1, filePath.Length - dir.Length - 1);

            if (!dicFileMD5.ContainsKey(key))
                dicFileMD5.Add(key, strMD5);
            else
                Debug.LogWarning("<Two File has the same name> name = " + filePath);
        }

        string savePath = System.IO.Path.Combine(Application.dataPath, "AssetBundle/") + platform + "/VersionNum";
        if (Directory.Exists(savePath) == false)
            Directory.CreateDirectory(savePath);

        // 删除前一版的数据
        if (File.Exists(savePath + "/VersionMD5-old.xml"))
        {
            System.IO.File.Delete(savePath + "/VersionMD5-old.xml");
        }

        // 如果之前的版本存在，则将其名字改为VersionMD5-old.xml
        if (File.Exists(savePath + "/VersionMD5.xml"))
        {
            System.IO.File.Move(savePath + "/VersionMD5.xml", savePath + "/VersionMD5-old.xml");
        }

        CreateXML(dicFileMD5, savePath);
    }


    /// <summary>
    /// 递归遍历文件夹下所有文件
    /// </summary>
    /// <param name="root"></param>
    /// <param name="fileList"></param>
    /// <returns></returns>
    public static List<string> GetAllFilePath(string root, List<string> fileList)
    {
        string[] filePaths = Directory.GetFiles(root);
        if (filePaths != null && filePaths.Length > 0)
        {
            foreach (string path in filePaths)
            {
                Debug.Log("GetAllFilePath:" + path);

                if (path.Contains(".meta") || path.Contains("VersionMD5") || path.Contains(".xml"))
                    continue;

                fileList.Add(path);
            }
        }

        string[] dirs = Directory.GetDirectories(root);
        if (dirs != null && dirs.Length > 0)
        {
            foreach (string dir in dirs)
            {
                GetAllFilePath(dir, fileList);
            }
        }

        return fileList;
    }

    //创建最新的 xml文件
    public static void CreateXML(Dictionary<string, string> dicFileMD5, string savePath)
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement xmlRoot = xmlDoc.CreateElement("Files");
        xmlDoc.AppendChild(xmlRoot);
        foreach (KeyValuePair<string, string> pair in dicFileMD5)
        {
            XmlElement xmlElem = xmlDoc.CreateElement("File");
            xmlRoot.AppendChild(xmlElem);

            xmlElem.SetAttribute("FileName", pair.Key);
            xmlElem.SetAttribute("MD5", pair.Value);
        }

        // 读取旧版本的MD5
        Dictionary<string, string> dicOldMD5 = ReadMD5File(savePath + "/VersionMD5-old.xml");

        // VersionMD5-old中有，而VersionMD5中没有的信息，添加到VersionMD5
        foreach (KeyValuePair<string, string> pair in dicOldMD5)
        {
            if (!dicFileMD5.ContainsKey(pair.Key))
                dicFileMD5.Add(pair.Key, pair.Value);
        }

        xmlDoc.Save(savePath + "/VersionMD5.xml");
        xmlDoc = null;
    }

    static Dictionary<string, string> ReadMD5File(string fileName)
    {
        Dictionary<string, string> dicMD5 = new Dictionary<string, string>();

        // 如果文件不存在，则直接返回
        if (System.IO.File.Exists(fileName) == false)
            return dicMD5;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(fileName);
        XmlElement xmlRoot = xmlDoc.DocumentElement;

        foreach (XmlNode node in xmlRoot.ChildNodes)
        {
            if ((node is XmlElement) == false)
                continue;

            string file = (node as XmlElement).GetAttribute("FileName");
            string md5 = (node as XmlElement).GetAttribute("MD5");

            if (dicMD5.ContainsKey(file) == false)
            {
                dicMD5.Add(file, md5);
            }
        }

        xmlRoot = null;
        xmlDoc = null;

        return dicMD5;
    }

}
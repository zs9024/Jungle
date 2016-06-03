//****************************************************************
// 创建日期：2015.11.7
// 文件名称：Extensions.cs
// 创建者  ：HANK
// 版权所有：CIFPAY
// 说明    ：字符串扩展方法
//****************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public static class StringExternsions
{
    public static void SaveText(this string text, String fileName)
    {
        if (!Directory.Exists(fileName.GetDirectoryName()))
        {
            Directory.CreateDirectory(fileName.GetDirectoryName());
        }
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }

        using (FileStream fs = new FileStream(fileName, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(text);
                sw.Flush();
                sw.Close();
            }
            fs.Close();
        }
    }

    public static string UnityAssetBundleName(this string str)
    {
        return string.Concat(str, ".unity3d");
    }
    public static string GetDirectoryName(this string fileName)
    {
        return fileName.Substring(0, fileName.LastIndexOf('/'));
    }
    
    
    public static byte[] LoadByteFile(this String fileName)
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
    
    public static String LoadFile(this String fileName)
    {
        if (File.Exists(fileName))
        {
            using (StreamReader sr = File.OpenText(fileName))
                return sr.ReadToEnd();
        }
        else
        {
            return String.Empty;
        }
            
    }

    public static String ReplaceFirst(this string input, string oldValue, string newValue, int startAt = 0)
    {
        int pos = input.IndexOf(oldValue, startAt);
        if (pos < 0)
        {
            return input;
        }
        return string.Concat(input.Substring(0, pos), newValue, input.Substring(pos + oldValue.Length));
    }

    public static String PackList<T>(this List<T> list, Char listSpriter = ',')
    {
        if (list.Count == 0)
        {
            return "";
        } 
        else
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.AppendFormat("{0}{1}", item, listSpriter);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

    }
    public static String PackArray<T>(this T[] array, Char listSpriter = ',')
    {
        var list = new List<T>();
        list.AddRange(array);
        return PackList(list, listSpriter);
    }

    public static string TrimSuffix(this string str)
    {
        str.Replace("\\", "/");
        if (-1 == str.LastIndexOf('/'))
        {
            if (-1 == str.LastIndexOf('.'))
            {
                return str;
            }
            else
            {
                return str.Substring(0, str.LastIndexOf('.'));
            }
        }
        else
        {
            if (-1 == str.LastIndexOf('.'))
            {
                return str.Substring(str.LastIndexOf('/') + 1);
            }
            else
            {
                return str.Substring(str.LastIndexOf('/') + 1, str.LastIndexOf('.'));
            }
        }
    }

    public static string FileName(this string str)
    {
        str.Replace("\\", "/");
        if (-1 == str.LastIndexOf('/'))
        {
            return str;
        }
        else
        {
            return str.Substring(str.LastIndexOf('/') + 1);
        }
    }
    
    //file or string ---> md5
    public static String MD5(this String str)
    {
        String md5 = String.Empty;
        //file md5
        if (System.IO.File.Exists(str))
        {
            using (System.IO.FileStream fileStream = System.IO.File.OpenRead(str))
            {
                System.Security.Cryptography.MD5 md5Creater = System.Security.Cryptography.MD5.Create();
                md5 = System.BitConverter.ToString(md5Creater.ComputeHash(fileStream)).Replace("-", "").ToLower();
            }
        }
        else//string md5
        {
            using (System.Security.Cryptography.MD5 md5Creater = System.Security.Cryptography.MD5.Create())
            {
                md5 = System.BitConverter.ToString(md5Creater.ComputeHash(System.Text.Encoding.Default.GetBytes(str))).Replace("-", "").ToLower();
            }
        }
        return md5;
    }
    //byte array ---> md5
    public static string MD5(this byte[] content)
    {
        string md5 = string.Empty;
        using (System.Security.Cryptography.MD5 md5Creater = System.Security.Cryptography.MD5.Create())
        {
            md5 = md5Creater.ComputeHash(content).ToString().Replace("-", "").ToLower();
        }
        return md5;
    }

    public static Vector3 String2Vector3(this string str)
    {
        if (str.IsNullOrEmpty())
        {
            return Vector3.zero;
        }

        string[] temp = str.Split(',');
        if(temp == null || temp.Length != 3)
        {
            return Vector3.zero;
        }

        return new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
    }
}
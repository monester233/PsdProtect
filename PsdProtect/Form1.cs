using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PsdProtect
{
    public partial class Form1 : Form
    {
        private string strRemPsd;                           //记忆密码
        private string strDiffWord;                         //区分代号
        private string strDescription;                      //代号描述
        private string strPsd;                              //最终密码

        public Form1()
        {
            InitializeComponent();
        }

        /*生成按钮点击事件响应*/
        private void button1_Click(object sender, EventArgs e)
        {
            /*判断合法性*/
            if (tbRemPsd.Text == "")
            {
                MessageBox.Show("请输入记忆密码!", "系统消息", MessageBoxButtons.OK);
                return;
            }
            if (tbDiffWord.Text == "")
            {
                MessageBox.Show("请输入区分代号！", "系统消息", MessageBoxButtons.OK);
                return;
            }
            /*根据记忆密码和区分代号生成加密原文*/
            string strOriginalPsd = "";
            strOriginalPsd = tbRemPsd.Text + "Monester" + tbDiffWord.Text;
            strPsd = encodeSHA1(strOriginalPsd, Encoding.UTF8);
            tbGenPsd.Text = strPsd;
            /*记录密码*/
            recordPsd();
        }

        /// <summary>
        /// 采用SHA1加密方式进行加密
        /// </summary>
        /// <param name="content">加密原文</param>
        /// <param name="encode">原文编码方式</param>
        /// <returns>加密密文</returns>
        private string encodeSHA1(string content, Encoding encode)
        {
            string strEncode = "";
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytesIn = encode.GetBytes(content);
                byte[] bytesOut = sha1.ComputeHash(bytesIn);
                sha1.Dispose();
                strEncode = BitConverter.ToString(bytesOut);
                strEncode = strEncode.Replace("-", "");
                return strEncode;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SHA1加密出错：" + ex.Message, "系统消息", MessageBoxButtons.OK);
                return strEncode;
            }
        }

        /// <summary>
        /// 采用MD5算法加密方式进行加密
        /// </summary>
        /// <param name="content">加密原文</param>
        /// <param name="encode">原文编码方式</param>
        /// <returns>加密密文</returns>
        private string encodeMD5(string content, Encoding encode)
        {
            string strEncode = "";
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] bytesIn = encode.GetBytes(content);
                byte[] bytesOut = md5.ComputeHash(bytesIn);
                md5.Dispose();
                strEncode = BitConverter.ToString(bytesOut);
                strEncode = strEncode.Replace("-", "");
                return strEncode;
            }
            catch (Exception ex)
            {
                MessageBox.Show("MD5加密出错：" + ex.Message, "系统消息", MessageBoxButtons.OK);
                return strEncode;
            }
        }

        /// <summary>
        /// 加密记录密码
        /// </summary>
        private void recordPsd()
        {
            string strFilePath = "F:\\psd.ini";
            FileStream file = new FileStream(strFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            MRSA mrsa = new MRSA();
            StreamReader reader = new StreamReader(file);
            string readBuf, writeBuf;
            /*解密私钥*/
            mrsa.RSAPrivateKey = reader.ReadLine();
            /*产生新的密钥对*/
            writeBuf = mrsa.genKey();
            /*循环读文件，查看是否存在对应密码*/
            while ((readBuf = reader.ReadLine()) != null)
            {
                readBuf = mrsa.decodeRSA(readBuf, Encoding.UTF8);

                if (readBuf == (tbRemPsd.Text + "Monester" + tbDiffWord.Text))   //以记录对应密码
                {
                    return;
                }
                else
                {
                    writeBuf += "\r\n" + mrsa.encodeRSA(readBuf, Encoding.UTF8);
                    readBuf = mrsa.decodeRSA(reader.ReadLine(), Encoding.UTF8);
                    writeBuf += "\r\n" + mrsa.encodeRSA(readBuf, Encoding.UTF8);
                }
            }
            /*加密新密码*/
            writeBuf += "\r\n" + mrsa.encodeRSA((tbRemPsd.Text + "Monester" + tbDiffWord.Text), Encoding.UTF8);
            writeBuf += "\r\n" + mrsa.encodeRSA(strPsd, Encoding.UTF8);
            /*对应密码未记录，记录*/
            file.Seek(0, SeekOrigin.Begin);
            StreamWriter writer = new StreamWriter(file);
            writer.Write(writeBuf);
            writer.Close();
            file.Close();
        }

        /// <summary>
        /// 是否显示记忆密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                tbRemPsd.PasswordChar = new char();
            }
            else
            {
                tbRemPsd.PasswordChar = '*';
            }
        }
    }
}

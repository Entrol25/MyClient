using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//
using System.Net;// Net 
using System.Net.Sockets;// сокеты 
using System.IO;// файлы
using System.Threading;// поток

namespace MyClient
{
    public partial class Form1 : Form
    {
        static private Socket Client;// сокет для клиента 
        private IPAddress ip = null;// тут храним IP 
        private int port = 0;// изначально порт = 0
        private Thread th;// создать поток

        public Form1()
        {
            InitializeComponent();
            //---------------------------------------------//
            // изначально заблокировано 
            richTextBox1.Enabled = false;//
            richTextBox2.Enabled = false;//
            button1.Enabled = false;//
            // считывать настройки клиента из файла 
            try// обработчик ошибок 
            {
                var sr = new StreamReader(@"Client_info/data_info.txt");// создать поток - читать из файла 
                string buffer = sr.ReadToEnd();// читать из файла 
                sr.Close();// остановить поток 
                // парсим 
                string[] connect_info = buffer.Split(':');// Split ищет разделитель : в массиве 
                ip = IPAddress.Parse(connect_info[0]);// ip стринг переводит в инт IPAddress.Parse
                port = int.Parse(connect_info[1]);// port стринг переводит в инт int.Parse 
                // если спарсилось отлично 
                label4.ForeColor = Color.Green;//
                label4.Text = "Настройки: \n IP сервера: " + connect_info[0] + "\n Порт сервера: " + connect_info[1];//
            }
            catch(Exception ex)// если из try ничего не сработало 
            {
                label4.ForeColor = Color.Red;//
                label4.Text = "Настройки не найдены!";//
                Form2 form = new Form2();// инициализация формы с настройками 
                form.Show();// показать форму с настройками 

            }

        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)// Настройки - меню 
        {
            Form2 form = new Form2();// инициализация формы с настройками
            form.Show();// показать форму с настройками 
        }

        void SendMessage(string message)// ф-я с параметром string. отправляет сообщения на сервер 
        {
            if(message != " " && message != "")// есть сообщение 
            {
                byte[] buffer = new byte[1024];// создать буфер байтов - массив 
                buffer = Encoding.UTF8.GetBytes(message);// получаем байт код 
                Client.Send(buffer);// отправляем на сервер
            }
        }

        void RecvMessage()// ф-я принимает сообщения от сервера 
        {
            byte[] buffer = new byte[1024];// массив - буфер байтов 
            for (int i = 0; i < buffer.Length; i++)// чистим буфер
            {
                buffer[i] = 0;// чистим буфер
            }

            for(; ; )// в цикле принимаются все сообщения 
            {
                try// обработчик ошибок 
                {
                    Client.Receive(buffer);// приём сообщения 
                    string message = Encoding.UTF8.GetString(buffer);// переводим байты в стринг 
                    int count = message.IndexOf(";;;5");// ищет ;;;5 - конец сообщения 
                    if (count == -1)// ищет ;;;5 - конец сообщения
                    {
                        continue;// продолжаем цикл  
                    }

                    string Clear_Message = "";// очищаем сообщение для работы 

                    for(int i = 0; i < count; i++)// 
                    {
                        Clear_Message += message[i];// добавляем буквы реального сообщения 
                    }
                    for (int i = 0; i < buffer.Length; i++)// чистим буфер
                    {
                        buffer[i] = 0;// чистим буфер
                    }
                    // ф-я исключения и делегат 
                    this.Invoke((MethodInvoker)delegate ()// мы в потоке. пишет в richTextBox1 
                    {
                        richTextBox1.AppendText(Clear_Message);// мы в потоке. пишет в richTextBox1 
                    });
                }
                catch(Exception ex)// если из try ничего не сработало 
                { }
            }
        }

        private void button2_Click(object sender, EventArgs e)// Вход - кнопка 
        {
            if (textBox1.Text != "" && textBox1.Text != " ")// если ник заполнен 
            {
                button1.Enabled = true;// включить 
                richTextBox2.Enabled = true;// включить 
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);// создать сокет 
                if(ip != null)// если IP найден 
                {
                    Client.Connect(ip, port);// подкл. к серверу 
                    th = new Thread(delegate () { RecvMessage(); });// создать поток 
                    th.Start();// запуск потока 
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)// Отправить - кнопка 
        {
            SendMessage("\n" + textBox1.Text + ": " + richTextBox2.Text + ";;;5");// ник и текст
            richTextBox2.Clear();// очистка 
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)// Выход - меню
        {
            if(th != null)// если есть поток 
            {
                th.Abort();// остановить поток 
            }
            if(Client != null)//  если работает клиент
            {
                Client.Close();// закрыть клиент 
            }
            
            Application.Exit();// выход 
        }
    }
}

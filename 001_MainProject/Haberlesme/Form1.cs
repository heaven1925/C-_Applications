using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Collections;
using System.Net;

namespace Haberlesme
{
    
    public partial class Form1 : Form
    {

        public Form1()

        {
            InitializeComponent();
        }

        // Global Değişken tanımlamaları

         public Socket Socket_Communication;
         public static EndPoint PC_EndPoint, Device_EndPoint;
         public static Int32 Byte_Total;
         public static byte[] Udp_GelenVeri;
         public static byte[] Receive;
         byte[] SendValue = new byte[15];  

         // point to point değişkenleri
         public static UInt16 X1Koor;
         public static UInt16 X2Koor;
         public static UInt32 YKoor;
         public static UInt16 ZKoor;

         public static double X1Koor_d;
         public static double X2Koor_d;
         public static double YKoor_d;
         public static double ZKoor_d;

         public static double X1Koor_m;
         public static double X2Koor_m;
         public static double YKoor_m;
         public static double ZKoor_m;


         public static byte X1_hiz;       
         public static byte X2_hiz;     
         public static byte Y_hiz;     
         public static byte Z_hiz;

         public static UInt16 Move_X1;
         public static UInt16 Move_X2;
         public static UInt32 Move_Y;
         public static UInt16 Move_Z;

         public static byte IN_0;
         public static byte IN_1;
         public static byte IN_2;
         public static byte IN_3;
         public static byte IN_4;
         public static byte IN_5;
         public static byte IN_6;
         public static byte IN_7;
         

         UInt16 koorVal = 0;
         byte speedVal = 0;

            // PortA
            UInt16 PortA_HighByte;         // ilk 8 bit kullanılacak
            UInt16 PortA_LowByte;          // son 8 bit kullanılacak
            UInt16 PortA_DataByte;         // 16 bit format

            // PortB
            UInt16 PortB_HighByte;         // ilk 8 bit kullanılacak
            UInt16 PortB_LowByte;          // son 8 bit kullanılacak
            UInt16 PortB_DataByte;         // 16 bit format

            // PortC
            UInt16 PortC_HighByte;         // ilk 8 bit kullanılacak
            UInt16 PortC_LowByte;          // son 8 bit kullanılacak
            UInt16 PortC_DataByte;         // 16 bit format

            // PortD
            UInt16 PortD_HighByte;         // ilk 8 bit kullanılacak
            UInt16 PortD_LowByte;          // son 8 bit kullanılacak
            UInt16 PortD_DataByte;         // 16 bit format

            // PortE
            UInt16 PortE_HighByte;         // ilk 8 bit kullanılacak
            UInt16 PortE_LowByte;          // son 8 bit kullanılacak
            UInt16 PortE_DataByte;         // 16 bit format

      
        // byte
            byte[] binaryArray = new byte[20]; // 0-15 16 bitlik 1 ve 0 değerleri  DataByte formatı = 1111 1111 1111 1111
           
            UInt16 DataByte = 0;

            UInt64 PortA_Data = 0;
            UInt64 PortB_Data = 0;
            UInt64 PortC_Data = 0;
            UInt64 PortD_Data = 0;
            UInt64 PortE_Data = 0;

            UInt64 incVal = 1;
            int arrayInc = 0; 


         // Interpolasyon degişkenleri
         byte caseVal_interpolasyon = 6;
         byte CRCVal_interpolasyon = 6;

         UInt16 koorVal_X1 = 0;
         UInt16 koorVal_X2 = 0;
         UInt16 koorVal_Y = 0;
         UInt16 koorVal_Z = 0;

         byte speedVal_int = 0;

  
        public class SeriHaberlesme
        {
            public static byte UDP_CRC_Hesapla(byte[] AraDizi, byte AraDataSayisi)      
            {

                byte Ara;
                byte i;
                Ara = AraDizi[0];
                for (i = 1; i <= AraDataSayisi + 1; i++)
                {
                    Ara = (byte)(Ara ^ AraDizi[i]);
                }

                Ara = (byte)~Ara;
                return Ara;
            }

            public static void UDP_GELEN_VERI_KONTROL()         // GelenData'nın 0. indisine göre hangi eksenin bilgisinin geldiğini kontrolü
            {
                
                switch (Udp_GelenVeri[0])
                {
                    case 3:
                        X1_KOOR_OKU();
                        break;
                    case 4:
                        X2_KOOR_OKU();
                        break;
                    case 5:
                        Y_KOOR_OKU();
                        break;
                    case 6:
                        Z_KOOR_OKU();
                        break;                         
                }
            }
        }
    
        private void OperatorCallback(IAsyncResult result)
        {
            byte Checksum;
            byte i;

            Byte_Total = Socket_Communication.EndReceiveFrom(result, ref Device_EndPoint);
            if (Byte_Total > 0)
            {

                Udp_GelenVeri = (Byte[])result.AsyncState;
                Checksum = SeriHaberlesme.UDP_CRC_Hesapla(Udp_GelenVeri, Udp_GelenVeri[1]);

                i = Udp_GelenVeri[1];
                if (Checksum == Udp_GelenVeri[i+2])
                {
                    SeriHaberlesme.UDP_GELEN_VERI_KONTROL();
                }
                else
                {
                    // ERROR
                }
                    
                
            }

            Socket_Communication.BeginReceiveFrom(Udp_GelenVeri, 0, Udp_GelenVeri.Length, SocketFlags.None, ref Device_EndPoint, new AsyncCallback(OperatorCallback), Udp_GelenVeri);
        }

        public static void X1_KOOR_OKU()        // Gelen X1 değerini Değişkene ATA
        {
            X1Koor = (UInt16)((Udp_GelenVeri[2]*256) + Udp_GelenVeri[3]);
            X1_hiz = (byte)(Udp_GelenVeri[4]);
            
        }

        public static void X2_KOOR_OKU()        // Gelen X2 değerini Değişkene ATA  
        {
            X2Koor = (UInt16)((Udp_GelenVeri[2] * 256) + Udp_GelenVeri[3]);
            X2_hiz = (byte)(Udp_GelenVeri[4]);
            
        }

        public static void Y_KOOR_OKU()         // Gelen y değerini Değişkene ATA
        {
            YKoor = (UInt16)((Udp_GelenVeri[2] * 256) + Udp_GelenVeri[3]);
            Y_hiz = (byte)(Udp_GelenVeri[4]);

        }

        public static void Z_KOOR_OKU()         // Gelen z değerini Değişkene ATA
        {
            ZKoor = (UInt16)((Udp_GelenVeri[2] * 256) + Udp_GelenVeri[3]);
            Z_hiz = (byte)(Udp_GelenVeri[4]);

        }


        private void button1_Click(object sender, EventArgs e)
        {
            // GELEN VERİLERİ RAW DATA KONTROLÜ
            timer2.Enabled = true;
            timer4.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)  // X1'e ait verileri gönderen button 
        {            
            // girilen değerler ekrandan okunacak.           
            koorVal = Convert.ToUInt16(textBox3.Text);
            speedVal = Convert.ToByte(textBox4.Text);
            // girilen değerleri göndermez dizisine taşınıyor.
            SendValue[0] = 2;
            SendValue[1] = 3;
            SendValue[2] = (byte)(koorVal >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(koorVal & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = speedVal;          
            // Gönderilecek değerler ekrana yazılıyor.
            label6.Text = SendValue[2].ToString();
            label11.Text = SendValue[3].ToString();
            label7.Text = koorVal.ToString();
            label8.Text = speedVal.ToString();    
            label7.BackColor = Color.Red;
            label8.BackColor = Color.Red;
            //Ethenet gönderme fonksiyonu      
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;     
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
          
        }

        private void button3_Click(object sender, EventArgs e) // // X2'e ait verileri gönderen button 
        {
            // girilen değerler ekrandan okunacak.           
            koorVal = Convert.ToUInt16(textBox3.Text);
            speedVal = Convert.ToByte(textBox4.Text);
            // girilen değerleri göndermez dizisine taşınıyor.
            SendValue[0] = 3;
            SendValue[1] = 3;
            SendValue[2] = (byte)(koorVal >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(koorVal & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = speedVal;
            // Gönderilecek değerler ekrana yazılıyor.
            label6.Text = SendValue[2].ToString();
            label11.Text = SendValue[3].ToString();
            label7.Text = koorVal.ToString();
            label8.Text = speedVal.ToString();
            label7.BackColor = Color.Red;
            label8.BackColor = Color.Red;
            //Ethenet gönderme fonksiyonu      
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void button6_Click(object sender, EventArgs e)      // Y eksenine ait veriler gönderiliyor.
        {
            // girilen değerler ekrandan okunacak.           
            koorVal = Convert.ToUInt16(textBox3.Text);
            speedVal = Convert.ToByte(textBox4.Text);
            // girilen değerleri göndermez dizisine taşınıyor.
            SendValue[0] = 4;
            SendValue[1] = 3;
            SendValue[2] = (byte)(koorVal >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(koorVal & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = speedVal;
            // Gönderilecek değerler ekrana yazılıyor.
            label6.Text = SendValue[2].ToString();
            label11.Text = SendValue[3].ToString();
            label7.Text = koorVal.ToString();
            label8.Text = speedVal.ToString();
            label7.BackColor = Color.Red;
            label8.BackColor = Color.Red;
            //Ethenet gönderme fonksiyonu      
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void button5_Click(object sender, EventArgs e)  // Z eksenine ait veriler gönderiliyor.
        {
            // girilen değerler ekrandan okunacak.           
            koorVal = Convert.ToUInt16(textBox3.Text);
            speedVal = Convert.ToByte(textBox4.Text);
            // girilen değerleri göndermez dizisine taşınıyor.
            SendValue[0] = 5;
            SendValue[1] = 3;
            SendValue[2] = (byte)(koorVal >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(koorVal & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = speedVal;
            // Gönderilecek değerler ekrana yazılıyor.
            label6.Text = SendValue[2].ToString();
            label11.Text = SendValue[3].ToString();
            label7.Text = koorVal.ToString();
            label8.Text = speedVal.ToString();
            label7.BackColor = Color.Red;
            label8.BackColor = Color.Red;
            //Ethenet gönderme fonksiyonu      
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)    // Ethernet UDP Init
        {
            try
            {
                Socket_Communication = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Socket_Communication.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                Socket_Communication.EnableBroadcast = true;

                PC_EndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.15"), Convert.ToInt32("2762"));
                Socket_Communication.Bind(PC_EndPoint);

                Device_EndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.14"), Convert.ToInt32("65100"));
                Socket_Communication.Connect(Device_EndPoint);

            }
            catch (Exception hata)
            {
                MessageBox.Show("Hata: " + "Baglanti Hatasi" + hata.ToString());
                Socket_Communication.Close();
            }
            Udp_GelenVeri = new Byte[30];
            Socket_Communication.BeginReceiveFrom(Udp_GelenVeri, 0, Udp_GelenVeri.Length, SocketFlags.None, ref Device_EndPoint, new AsyncCallback(OperatorCallback), Udp_GelenVeri);
            timer1.Enabled = false;

        }

        

        private void timer2_Tick(object sender, EventArgs e)     // Data Receive Timer
        {
            
            // gönderilen data'nın 0. indisine göre case seçimi yapılıyor.
            // ARM Tarafından SendData fonksiyonunun 0. indisine bağlı 
           
            switch(Udp_GelenVeri[0])                                            
            {
                case 1:
                    PortA_HighByte =  Udp_GelenVeri[2];    // A LOW
                    PortA_LowByte  =  Udp_GelenVeri[3];    // A LOW

                    PortB_HighByte = Udp_GelenVeri[4];   // B HIGH
                    PortB_LowByte  = Udp_GelenVeri[5];   // B LOW

                    PortC_HighByte = Udp_GelenVeri[6];   // C HIGH
                    PortC_LowByte  = Udp_GelenVeri[7];   // C LOW

                    PortD_HighByte = Udp_GelenVeri[8];  // D HIGH
                    PortD_LowByte  = Udp_GelenVeri[9];  // D LOW

                    PortE_HighByte = Udp_GelenVeri[10];  // E HIGH
                    PortE_LowByte  = Udp_GelenVeri[11];  // E LOW

                    PortA_DataByte = (UInt16)((PortA_HighByte << 8) + (PortA_LowByte & 0xFF));
                    PortB_DataByte = (UInt16)((PortB_HighByte << 8) + (PortB_LowByte & 0xFF));
                    PortC_DataByte = (UInt16)((PortC_HighByte << 8) + (PortC_LowByte & 0xFF));
                    PortD_DataByte = (UInt16)((PortD_HighByte << 8) + (PortD_LowByte & 0xFF));
                    PortE_DataByte = (UInt16)((PortE_HighByte << 8) + (PortE_LowByte & 0xFF));

                    // 2 Byte to binary configuration


                    // PORT A DATA FORMAT
                    DataByte = (UInt16)PortA_DataByte;
                    label96.Text = ((byte)(DataByte % 2)).ToString();  // 0.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0111 1111 1111 1111                    
                    label97.Text = ((byte)(DataByte % 2)).ToString();  // 1.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0011 1111 1111 1111
                    label98.Text = ((byte)(DataByte % 2)).ToString();  // 2.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0001 1111 1111 1111
                    label99.Text = ((byte)(DataByte % 2)).ToString();  // 3.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 1111 1111 1111
                    label100.Text = ((byte)(DataByte % 2)).ToString();  // 4.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0111 1111 1111
                    label101.Text = ((byte)(DataByte % 2)).ToString();  // 5.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0011 1111 1111
                    label102.Text = ((byte)(DataByte % 2)).ToString();  // 6.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0001 1111 1111
                    label103.Text = ((byte)(DataByte % 2)).ToString();  // 7.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 1111 1111
                    label86.Text = ((byte)(DataByte % 2)).ToString();  // 8.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0111 1111
                    label93.Text = ((byte)(DataByte % 2)).ToString();  // 9.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0011 1111
                    label94.Text = ((byte)(DataByte % 2)).ToString(); // 10.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0001 1111
                    label95.Text = ((byte)(DataByte % 2)).ToString(); // 11.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 1111
                    label84.Text = ((byte)(DataByte % 2)).ToString(); // 12.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0111
                    label85.Text = ((byte)(DataByte % 2)).ToString(); // 13.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0011
                    label83.Text = ((byte)(DataByte % 2)).ToString(); // 14.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0001
                    label82.Text = ((byte)(DataByte % 2)).ToString(); // 15.bit
                    // PORT B DATA FORMAT
                    DataByte = (UInt16)PortB_DataByte;
                    label104.Text = ((byte)(DataByte % 2)).ToString();  // 0.bit  
                    IN_0 = ((byte)(DataByte % 2));
                    DataByte = (UInt16)(DataByte / 2);       // 0111 1111 1111 1111                    
                    label105.Text = ((byte)(DataByte % 2)).ToString();  // 1.bit  
                    IN_1 = ((byte)(DataByte % 2));
                    DataByte = (UInt16)(DataByte / 2);       // 0011 1111 1111 1111
                    label106.Text = ((byte)(DataByte % 2)).ToString();  // 2.bit  
                    IN_2 = ((byte)(DataByte % 2));
                    DataByte = (UInt16)(DataByte / 2);       // 0001 1111 1111 1111
                    label107.Text = ((byte)(DataByte % 2)).ToString();  // 3.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 1111 1111 1111
                    label108.Text = ((byte)(DataByte % 2)).ToString();  // 4.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0111 1111 1111
                    label109.Text = ((byte)(DataByte % 2)).ToString();  // 5.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0011 1111 1111
                    label110.Text = ((byte)(DataByte % 2)).ToString();  // 6.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0001 1111 1111
                    label111.Text = ((byte)(DataByte % 2)).ToString();  // 7.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 1111 1111
                    label112.Text = ((byte)(DataByte % 2)).ToString();  // 8.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0111 1111
                    label113.Text = ((byte)(DataByte % 2)).ToString();  // 9.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0011 1111
                    label114.Text = ((byte)(DataByte % 2)).ToString(); // 10.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0001 1111
                    label115.Text = ((byte)(DataByte % 2)).ToString(); // 11.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 1111
                    label116.Text = ((byte)(DataByte % 2)).ToString(); // 12.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0111
                    label117.Text = ((byte)(DataByte % 2)).ToString(); // 13.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0011
                    label118.Text = ((byte)(DataByte % 2)).ToString(); // 14.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0001
                    label119.Text = ((byte)(DataByte % 2)).ToString(); // 15.bit
                    // PORT C DATA FORMAT
                    DataByte = (UInt16)PortC_DataByte;
                    label120.Text = ((byte)(DataByte % 2)).ToString();  // 0.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0111 1111 1111 1111                    
                    label121.Text = ((byte)(DataByte % 2)).ToString();  // 1.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0011 1111 1111 1111
                    label122.Text = ((byte)(DataByte % 2)).ToString();  // 2.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0001 1111 1111 1111
                    label123.Text = ((byte)(DataByte % 2)).ToString();  // 3.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 1111 1111 1111
                    label124.Text = ((byte)(DataByte % 2)).ToString();  // 4.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0111 1111 1111
                    label125.Text = ((byte)(DataByte % 2)).ToString();  // 5.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0011 1111 1111
                    label126.Text = ((byte)(DataByte % 2)).ToString();  // 6.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0001 1111 1111
                    label127.Text = ((byte)(DataByte % 2)).ToString();  // 7.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 1111 1111
                    label128.Text = ((byte)(DataByte % 2)).ToString();  // 8.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0111 1111
                    label129.Text = ((byte)(DataByte % 2)).ToString();  // 9.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0011 1111
                    label130.Text = ((byte)(DataByte % 2)).ToString(); // 10.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0001 1111
                    label131.Text = ((byte)(DataByte % 2)).ToString(); // 11.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 1111
                    label132.Text = ((byte)(DataByte % 2)).ToString(); // 12.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0111
                    label133.Text = ((byte)(DataByte % 2)).ToString(); // 13.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0011
                    label134.Text = ((byte)(DataByte % 2)).ToString(); // 14.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0001
                    label135.Text = ((byte)(DataByte % 2)).ToString(); // 15.bit
                    // PORT D DATA FORMAT
                    DataByte = (UInt16)PortD_DataByte;
                    label136.Text = ((byte)(DataByte % 2)).ToString();  // 0.bit  
                    IN_7 = ((byte)(DataByte % 2));
                    DataByte = (UInt16)(DataByte / 2);       // 0111 1111 1111 1111                    
                    label137.Text = ((byte)(DataByte % 2)).ToString();  // 1.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0011 1111 1111 1111
                    label138.Text = ((byte)(DataByte % 2)).ToString();  // 2.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0001 1111 1111 1111
                    label139.Text = ((byte)(DataByte % 2)).ToString();  // 3.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 1111 1111 1111
                    label140.Text = ((byte)(DataByte % 2)).ToString();  // 4.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0111 1111 1111
                    label141.Text = ((byte)(DataByte % 2)).ToString();  // 5.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0011 1111 1111
                    label142.Text = ((byte)(DataByte % 2)).ToString();  // 6.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0001 1111 1111
                    label143.Text = ((byte)(DataByte % 2)).ToString();  // 7.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 1111 1111
                    label144.Text = ((byte)(DataByte % 2)).ToString();  // 8.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0111 1111
                    label145.Text = ((byte)(DataByte % 2)).ToString();  // 9.bit
                    IN_3 = ((byte)(DataByte % 2));
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0011 1111
                    label146.Text = ((byte)(DataByte % 2)).ToString(); // 10.bit
                    IN_4 = ((byte)(DataByte % 2));
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0001 1111
                    label147.Text = ((byte)(DataByte % 2)).ToString(); // 11.bit
                    IN_5 = ((byte)(DataByte % 2));
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 1111
                    label148.Text = ((byte)(DataByte % 2)).ToString(); // 12.bit
                    IN_6 = ((byte)(DataByte % 2));
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0111
                    label149.Text = ((byte)(DataByte % 2)).ToString(); // 13.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0011
                    label150.Text = ((byte)(DataByte % 2)).ToString(); // 14.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0001
                    label151.Text = ((byte)(DataByte % 2)).ToString(); // 15.bit
                    // PORT E DATA FORMAT
                    DataByte = (UInt16)PortE_DataByte;
                    label152.Text = ((byte)(DataByte % 2)).ToString();  // 0.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0111 1111 1111 1111                    
                    label153.Text = ((byte)(DataByte % 2)).ToString();  // 1.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0011 1111 1111 1111
                    label154.Text = ((byte)(DataByte % 2)).ToString();  // 2.bit  
                    DataByte = (UInt16)(DataByte / 2);       // 0001 1111 1111 1111
                    label155.Text = ((byte)(DataByte % 2)).ToString();  // 3.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 1111 1111 1111
                    label156.Text = ((byte)(DataByte % 2)).ToString();  // 4.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0111 1111 1111
                    label157.Text = ((byte)(DataByte % 2)).ToString();  // 5.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0011 1111 1111
                    label158.Text = ((byte)(DataByte % 2)).ToString();  // 6.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0001 1111 1111
                    label159.Text = ((byte)(DataByte % 2)).ToString();  // 7.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 1111 1111
                    label160.Text = ((byte)(DataByte % 2)).ToString();  // 8.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0111 1111
                    label161.Text = ((byte)(DataByte % 2)).ToString();  // 9.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0011 1111
                    label162.Text = ((byte)(DataByte % 2)).ToString(); // 10.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0001 1111
                    label163.Text = ((byte)(DataByte % 2)).ToString(); // 11.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 1111
                    label164.Text = ((byte)(DataByte % 2)).ToString(); // 12.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0111
                    label165.Text = ((byte)(DataByte % 2)).ToString(); // 13.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0011
                    label166.Text = ((byte)(DataByte % 2)).ToString(); // 14.bit
                    DataByte = (UInt16)(DataByte / 2);       // 0000 0000 0000 0001
                    label167.Text = ((byte)(DataByte % 2)).ToString(); // 15.bit

                   if(IN_0 == 1)
                   {
                       label190.BackColor = Color.Black;
                   }
                   else
                   {                  
                       label190.BackColor = Color.Red;
                   }

                   if (IN_1 == 1)
                   {
                       label191.BackColor = Color.Black;
                   }
                   else
                   {
                       label191.BackColor = Color.Red;
                   }

                   if (IN_2 == 1)
                   {
                       label192.BackColor = Color.Black;
                   }
                   else
                   {
                       label190.BackColor = Color.Red;
                   }

                   if (IN_3 == 1)
                   {
                       label193.BackColor = Color.Black;
                   }
                   else
                   {
                       label190.BackColor = Color.Red;
                   }

                   if (IN_4 == 1)
                   {
                       label194.BackColor = Color.Black;
                   }
                   else
                   {
                       label194.BackColor = Color.Red;
                   }

                   if (IN_5 == 1)
                   {
                       label195.BackColor = Color.Black;
                   }
                   else
                   {
                       label195.BackColor = Color.Red;
                   }

                   if (IN_6 == 1)
                   {
                       label196.BackColor = Color.Black;
                   }
                   else
                   {
                       label196.BackColor = Color.Red;
                   }

                   if (IN_7 == 1)
                   {
                       label197.BackColor = Color.Black;
                   }
                   else
                   {
                       label197.BackColor = Color.Red;
                   }

            
            break;
                case 2:
                    // DURUM
                    break;
                case 3:
                    // X1 Ekseni
                    X1Koor = (UInt16)((Udp_GelenVeri[2] * 256) + Udp_GelenVeri[3]);
                    X1_hiz = (byte)(Udp_GelenVeri[4]);
                    label12.Text = X1Koor.ToString();                  
                    label14.Text = X1_hiz.ToString();

                    X1Koor_d = Convert.ToDouble(X1Koor);
                    X1Koor_m = X1Koor_d * 0.0017;
                    label187.Text = X1Koor_m.ToString();

                        break;
                case 4:
                    // X2 Ekseni
                    X2Koor = (UInt16)((Udp_GelenVeri[2] * 256) + Udp_GelenVeri[3]);
                    X2_hiz = (byte)(Udp_GelenVeri[4]);
                    label35.Text = X2Koor.ToString();
                    label43.Text = X2_hiz.ToString();

                    X2Koor_d = Convert.ToDouble(X2Koor);
                    X2Koor_m = X2Koor_d * 0.0017;
                    label184.Text = X2Koor_m.ToString();

                    break;
                case 5:
                    // Y Ekseni
                    YKoor = (UInt32)((Udp_GelenVeri[2]*256) + Udp_GelenVeri[3]);
                    Y_hiz = (byte)(Udp_GelenVeri[4]);
                    label13.Text = YKoor.ToString();
                    label15.Text = Y_hiz.ToString();

                    YKoor_d = Convert.ToDouble(YKoor);
                    YKoor_m = YKoor_d * 0.0017;
                    label186.Text = YKoor_m.ToString();
                    break;
                case 6:
                    // Z Ekseni
                    ZKoor = (UInt16)((Udp_GelenVeri[2] * 256) + Udp_GelenVeri[3]);
                    Z_hiz = (byte)(Udp_GelenVeri[4]);
                    label34.Text = ZKoor.ToString();

                    label42.Text = Z_hiz.ToString();

                    ZKoor_d = Convert.ToDouble(ZKoor);
                    ZKoor_m = ZKoor_d * 0.0017;
                    label185.Text = ZKoor_m.ToString();

                    break;
                 
            }       
        }
 
        private void button4_Click(object sender, EventArgs e)   // Interpolasyon için veri gönderimi
        {
            // Gönderilecek veri değişkenlere atanıyor.
            koorVal_X1 = Convert.ToUInt16(textBox1.Text);
            koorVal_X2 = Convert.ToUInt16(textBox2.Text);
            koorVal_Y = Convert.ToUInt16(textBox6.Text);

            speedVal_int = Convert.ToByte(textBox5.Text);


            // Hangi eksen master ekranda yazıyor.
            if (koorVal_X1 > koorVal_X2 && koorVal_X1 > koorVal_Y)
            {
                label41.Text = " X1 ";
                label41.BackColor = Color.Red;             
            }
            else if(koorVal_X2 > koorVal_X1 && koorVal_X2 > koorVal_Y)
            {
                label41.Text = " X2 ";
                label41.BackColor = Color.Red;  
            }
            else if (koorVal_Y > koorVal_X1 && koorVal_Y > koorVal_X2)
            {
                label41.Text = " Y ";
                label41.BackColor = Color.Red;
            }
            else
            {
                label41.Text = " - ";
                label41.BackColor = Color.Red;
            }
            // ARM TARAFINA INTERPOLASYON ICIN DEGER GONDERİLDİGİ BELLİ EDİLİYOR.
            // CASE:6 INTERPOLASYON
            SendValue[0] = caseVal_interpolasyon;
            SendValue[1] = CRCVal_interpolasyon;
            SendValue[2] = (byte)(koorVal_X1 >> 8);
            SendValue[3] = (byte)(koorVal_X1 & 0x00FF);
            SendValue[4] = (byte)(koorVal_X2 >> 8); ;
            SendValue[5] = (byte)(koorVal_X2 & 0x00FF);
            SendValue[6] = (byte)(koorVal_Y >> 8); ;
            SendValue[7] = (byte)(koorVal_Y & 0x00FF);
            SendValue[8] = speedVal_int;

            // VERİLER GÖNDERİLİYOR
            try
            {
                SendValue[9] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;    
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }

        }

  

        private void timer4_Tick(object sender, EventArgs e)
        {
            // Gelen veriler raw data olarak ekranda gösteriliyor.
            label16.Text = Udp_GelenVeri[0].ToString();
            label17.Text = Udp_GelenVeri[1].ToString();
            label18.Text = Udp_GelenVeri[2].ToString();
            label19.Text = Udp_GelenVeri[3].ToString();
            label20.Text = Udp_GelenVeri[4].ToString();
            label21.Text = Udp_GelenVeri[5].ToString();
            // Giden veriler raw data olarak ekranda gösteriliyor.
            label59.Text = SendValue[0].ToString();
            label58.Text = SendValue[1].ToString();
            label57.Text = SendValue[2].ToString();
            label56.Text = SendValue[3].ToString();
            label55.Text = SendValue[4].ToString();
            label54.Text = SendValue[5].ToString();
            label63.Text = SendValue[6].ToString();
            label62.Text = SendValue[7].ToString();
            label65.Text = SendValue[8].ToString();
        }

        private void button7_Click(object sender, EventArgs e)      // MOVE X1 ++
        {
            SendValue[0] = 2;
            SendValue[1] = 3;
            Move_X1 += 200;
            SendValue[2] = (byte)(Move_X1 >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(Move_X1 & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = 20;
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void button11_Click(object sender, EventArgs e)      // MOVE X1 --
        {
            SendValue[0] = 2;
            SendValue[1] = 3;
            Move_X1 -= 200;
            SendValue[2] = (byte)(Move_X1 >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(Move_X1 & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = 20;
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void button8_Click(object sender, EventArgs e) // MOVE X2 ++
        {
            SendValue[0] = 3;
            SendValue[1] = 3;
            Move_X2 += 200;
            SendValue[2] = (byte)(Move_X2 >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(Move_X2 & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = 20;
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void button12_Click(object sender, EventArgs e) // MOVE X2 --
        {
            SendValue[0] = 3;
            SendValue[1] = 3;
            Move_X2 -= 200;
            SendValue[2] = (byte)(Move_X2 >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(Move_X2 & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = 20;
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void button9_Click(object sender, EventArgs e) // MOVE Y ++
        {
            SendValue[0] = 4;
            SendValue[1] = 3;
            Move_Y += 200;
            SendValue[2] = (byte)(Move_Y >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(Move_Y & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = 20;
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void button13_Click(object sender, EventArgs e) // MOVE Y --
        {
            SendValue[0] = 4;
            SendValue[1] = 3;
            Move_Y -= 200;
            SendValue[2] = (byte)(Move_Y >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(Move_Y & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = 20;
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void button10_Click(object sender, EventArgs e) // MOVE Z++
        {
            SendValue[0] = 5;
            SendValue[1] = 3;
            Move_Z += 200;
            SendValue[2] = (byte)(Move_Z >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(Move_Z & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = 20;
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            SendValue[0] = 5;
            SendValue[1] = 3;
            Move_Z -= 200;
            SendValue[2] = (byte)(Move_Z >> 8);              // Low Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[3] = (byte)(Move_Z & 0x00FF);          // High Byte, gönderilecek değer 2 byte formatına çeviriliyor.
            SendValue[4] = 20;
            try
            {
                SendValue[5] = SeriHaberlesme.UDP_CRC_Hesapla(SendValue, SendValue[1]);
                Socket_Communication.Send(SendValue);
                return;
            }
            catch (Exception hataXPoz)
            {
                MessageBox.Show("ERR");
            }
        }


        


      
       
       
       

        

       

        

      

        

        

      
       

   

  
    }
}

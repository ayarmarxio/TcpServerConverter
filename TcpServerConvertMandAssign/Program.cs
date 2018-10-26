using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeightConverterLib;
//using System.Runtime.dll, mscorlib.dll, netstandard.dll


namespace TcpServerConvertMandAssign
{
    class Program
    {
        private static TcpClient _connectionSocket = null;
        private static TcpListener _serverSocket = null;
        private static IPAddress _ip = IPAddress.Parse("127.0.0.1");
        private static int _portNumber = 7777;
        private static Stream _nstream = null;
        private static StreamWriter _sWriter = null;
        private static StreamReader _sReader = null;
        private static string _msgFromClient = null;

        static void Main(string[] args)
        {
            try
            {
                // Step no: 2..............................................
                // create handshake , then welcoming server socket
                
                _serverSocket = new TcpListener(_ip, _portNumber);

                // Start listening incoming request from client 

                _serverSocket.Start();

                Console.WriteLine("Server is being start");
                Console.WriteLine("Ready for Handshake Call from Client");

                while (true)
                {
                    try
                    {
                        _connectionSocket = _serverSocket.AcceptTcpClient();
                        Console.WriteLine("Server is activated");
                        Task.Run(() => DoIt(_connectionSocket));
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Socket exception: Will continue working");
                    }
                }                                
                //_serverSocket.Stop();                    
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        } 
        
        private static void DoIt(TcpClient clientConnection)
        {

            Console.WriteLine("Incoming client " + clientConnection.Client);
          
            // Step no : 4 ...........................................
            // Server recieved (byte of data) from client , Server perf orm read opertion
            _nstream = _connectionSocket.GetStream();

            while (_nstream != null)
            {
                _sReader = new StreamReader(_nstream);

                _msgFromClient = _sReader.ReadLine();
                int position = _msgFromClient.IndexOf("=");
                string firstValue = _msgFromClient.Substring(0, position);
                string secondValue = _msgFromClient.Substring(position + 1);
                double valueFromClient = double.Parse(secondValue);

                _sWriter = new StreamWriter(_nstream) { AutoFlush = true };

                if ("g" == firstValue)
                {
                    WeightConverterLibClass weightConverter = new WeightConverterLibClass();
                    double result = weightConverter.fromGramToOunces(valueFromClient);
                    _sWriter.WriteLine(result);
                    Console.WriteLine("Client Msg:" + result);
                }
                else
                {
                    WeightConverterLibClass weightConverter = new WeightConverterLibClass();
                    double result = weightConverter.fromOuncesToGrams(valueFromClient);
                    _sWriter.WriteLine(result);
                    Console.WriteLine("Client Msg:" + result);
                }
            }

            
        }
    }
}

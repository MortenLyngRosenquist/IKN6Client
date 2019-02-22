using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp
{
	class file_client
	{
		const int PORT = 9000;
		const int BUFSIZE = 1000;
     
        TcpClient clientSocket; 
              
		private string ServerIP;
		private string filePathFromServer;
		private string filePathToSave = "/root/IKN6/Exercise_6_c#/Exercise_6_c#/file_client/bin/Debug/";
		private string file;

        private file_client (string[] args)
		{
            #if DEBUG
			ConfiguratePathAndIp(new string[] { "10.0.0.1", "/root/Hej.txt/" });
            #endif

			ConfiguratePathAndIp(args);

			NetworkStream io = EstablishConnection();
            
            
            receiveFile(filePathFromServer,io);
        }

		bool ValidateFile(NetworkStream io)
		{
			LIB.writeTextTCP(io, filePathFromServer);
			Console.WriteLine($" << Wrote to server {filePathFromServer}");
			return LIB.readTextTCP(io) == "Valid";
		}
		NetworkStream EstablishConnection()
		{
			clientSocket = new TcpClient(ServerIP, PORT);
            Console.WriteLine(" << Client Started");
            Console.WriteLine(" << Client Socket Program - Server Connected");
			return clientSocket.GetStream();
		}

		void ConfiguratePathAndIp(string[] args)
		{
			if (args[0] != null)
                ServerIP = args[0];
            else
                Console.WriteLine(" << Write an ip in input argument");

            if (args[1] != null)
                filePathFromServer = args[1];
            else
                Console.WriteLine(" << Write an path and file in input argument");

			file = LIB.extractFileName(filePathFromServer);
            filePathToSave += file;
		}
        
		private void ShutDownConnection(NetworkStream io)
		{
			io.Close();
            clientSocket.Close();

            Console.WriteLine(" << Connection is shut down");
			Console.WriteLine("----------------------");
			Console.WriteLine("");
		}

        private void receiveFile (String fileName, NetworkStream io)
		{         
			Byte[] fileBytes;

			if(ValidateFile(io))
			{
				long fileSize;
                fileSize = LIB.getFileSizeTCP(io);
                Console.WriteLine($" << File Size is: {fileSize}");

				fileBytes = new Byte[fileSize]; 
                int offset = 0;
                for (int i = (int)fileSize; i != 0;)
                {
                    if(i > BUFSIZE)
                    {
                        io.Read(fileBytes, offset, BUFSIZE);
                        offset += BUFSIZE;
                        i -= BUFSIZE;
                    }
                    else
                    {
                        io.Read(fileBytes, offset, i);
                        i -= i;
                    }
					Console.WriteLine($"{offset}, {i}");
                }  
				File.WriteAllBytes(filePathToSave, fileBytes);            
                Console.WriteLine($" << File is saved: {filePathToSave}");
			}
			else
			{
				Console.WriteLine(" << You asked for a invalid file");	
			}         
			ShutDownConnection(io);    
        }
        
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			new file_client(args);
		}
	}
}

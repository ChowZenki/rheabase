using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CommonLib;
using CommonLib.Net;

namespace AccountServer
{
    public class AccountServerClient : NetworkConnectionBase, IPacketLenghtDatabase
    {
        private static readonly log4net.ILog _Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int _AccountID;
        public int AccountID
        {
            get { return _AccountID; }
        }

        private Sex _Sex;
        public Sex Sex
        {
            get { return _Sex; }
        }

        public AccountServerClient(Socket client)
            : base(client)
        {
            PacketDatabase = this;
        }

        protected override void OnPacketReceived(ushort packetID, int packetSize, BinaryBuffer packetData)
        {
            switch (packetID)
            {
                case 0x64:
                    ProcessLoginPacket(packetID, packetData);
                    break;
                case 0x277:
                    ProcessLoginPacket(packetID, packetData);
                    break;
                case 0x2b0:
                    ProcessLoginPacket(packetID, packetData);
                    break;
                case 0x1dd:
                    ProcessLoginPacket(packetID, packetData);
                    break;
                case 0x1fa:
                    ProcessLoginPacket(packetID, packetData);
                    break;
                case 0x27c:
                    ProcessLoginPacket(packetID, packetData);
                    break;
                case 0x825:
                    ProcessLoginTokenPacket(packetID, packetData);
                    break;
            }
        }

        private void ProcessLoginPacket(ushort packetID, BinaryBuffer buffer)
        {
            bool isRaw = (packetID == 0x64 || packetID == 0x277 || packetID == 0x2b0);
            string username;
            int version;
            byte clientType;

            version = buffer.ReadInt32(0);
            username = buffer.ReadCString(4, 24);

            if (isRaw)
            {
                string password = buffer.ReadCString(28, 24);
                clientType = buffer.ReadByte(52);

                _Log.InfoFormat("Request for connection of {0} from {1}", username, Client.RemoteEndPoint);

                int errorCode;
                bool result = AccountServer.Instance.Authenticate(username, password, version, clientType, out errorCode, out _AccountID, out _Sex);

                if (result)
                    ReplyLoginOk();
                else
                    ReplyLoginError(username, errorCode);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ProcessLoginTokenPacket(ushort packetID, BinaryBuffer buffer)
        {
            throw new NotImplementedException();
        }

        private void ReplyLoginOk()
        {
            throw new NotImplementedException();
        }

        private void ReplyLoginError(string username, int errorCode)
        {
            string error = "";

            switch (errorCode)
            {
                case 0: error = "Unregistered ID."; break; // 0 = Unregistered ID
                case 1: error = "Incorrect Password."; break; // 1 = Incorrect Password
                case 2: error = "Account Expired."; break; // 2 = This ID is expired
                case 3: error = "Rejected from server."; break; // 3 = Rejected from Server
                case 4: error = "Blocked by GM."; break; // 4 = You have been blocked by the GM Team
                case 5: error = "Not latest game EXE."; break; // 5 = Your Game's EXE file is not the latest version
                case 6: error = "Banned."; break; // 6 = Your are Prohibited to log in until %s
                case 7: error = "Server Over-population."; break; // 7 = Server is jammed due to over populated
                case 8: error = "Account limit from company"; break; // 8 = No more accounts may be connected from this company
                case 9: error = "Ban by DBA"; break; // 9 = MSI_REFUSE_BAN_BY_DBA
                case 10: error = "Email not confirmed"; break; // 10 = MSI_REFUSE_EMAIL_NOT_CONFIRMED
                case 11: error = "Ban by GM"; break; // 11 = MSI_REFUSE_BAN_BY_GM
                case 12: error = "Working in DB"; break; // 12 = MSI_REFUSE_TEMP_BAN_FOR_DBWORK
                case 13: error = "Self Lock"; break; // 13 = MSI_REFUSE_SELF_LOCK
                case 14: error = "Not Permitted Group"; break; // 14 = MSI_REFUSE_NOT_PERMITTED_GROUP
                case 15: error = "Not Permitted Group"; break; // 15 = MSI_REFUSE_NOT_PERMITTED_GROUP
                case 99: error = "Account gone."; break; // 99 = This ID has been totally erased
                case 100: error = "Login info remains."; break; // 100 = Login information remains at %s
                case 101: error = "Hacking investigation."; break; // 101 = Account has been locked for a hacking investigation. Please contact the GM Team for more information
                case 102: error = "Bug investigation."; break; // 102 = This account has been temporarily prohibited from login due to a bug-related investigation
                case 103: error = "Deleting char."; break; // 103 = This character is being deleted. Login is temporarily unavailable for the time being
                case 104: error = "Deleting spouse char."; break; // 104 = This character is being deleted. Login is temporarily unavailable for the time being
                default: error = "Unknown Error."; break;
            }

            _Log.InfoFormat("Login for {0} refused: {1}", username, error);

            BinaryBuffer blob = new BinaryBuffer(23);
            blob.WriteInt16(0, 0x6a);
            blob.WriteByte(2, (byte)errorCode);

            if (errorCode == 6)
            {
                DateTime? time = (from acc in AccountServer.Instance.GetDataContext().Accounts where acc.Username == username select acc.BanTime).FirstOrDefault();
                
                if (time.HasValue)
                    blob.WriteCString(3, time.Value.ToString(), 20);
            }

            Send(blob);
        }

        int IPacketLenghtDatabase.GetPacketLenght(ushort packetID)
        {
            switch (packetID)
            {
                case 0x64: return 55;
                case 0x277: return 84;
                case 0x2b0: return 85;
                case 0x1dd: return 47;
                case 0x1fa: return 48;
                case 0x27c: return 60;
                case 0x825: return -1;
                case 0x204: return 18;
            }

            return 0;
        }
    }
}

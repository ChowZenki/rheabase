using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AccountServer;
using CommonLib;
using CommonLib.Configuration;
using CommonLib.Net;
using CommonLib.Remoting;
using MySql.Data.MySqlClient;

namespace CharServer
{
    public class CharServer : INetworkConnectionFactory
    {
        private static readonly log4net.ILog _Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static CharServer Instance { get; private set; }

        private bool _Running;

        private readonly PropertyTree _ConfigTree;
        public PropertyTree ConfigTree
        {
            get { return _ConfigTree; }
        }

        private readonly ConnectionListener _Listener;
        public ConnectionListener Listener
        {
            get { return _Listener; }
        }

        private IDbConnection _DatabaseConnection;
        public IDbConnection DatabaseConnection
        {
            get { return _DatabaseConnection; }
        }

        private RemotingServer _RemotingServer;
        public RemotingServer RemotingServer
        {
            get { return _RemotingServer; }
        }

        private RemotingClient _RemotingClient;
        public RemotingClient RemotingClient
        {
            get { return _RemotingClient; }
        }

        private IRemotedObject<ICharServerService> _Service;
        public IRemotedObject<ICharServerService> Service
        {
            get { return _Service; }
        }

        public CharServer()
        {
            Instance = this;
            _ConfigTree = new PropertyTree(StringComparer.InvariantCultureIgnoreCase);
            _Listener = new ConnectionListener(this);
        }

        public void Run()
        {
            _Log.Info("CharServer is starting");

            _Log.Info("Reading configuration");
            _ConfigTree.FromConfigFile("Config/CharServer.cfg");

            _Log.Info("Connecting to database");
            _DatabaseConnection = new MySqlConnection(BuildConnectionString());

            try
            {
                _DatabaseConnection.Open();
            }
            catch (Exception ex)
            {
                _Log.Fatal("Error connecting to database!", ex);
                return;
            }

            IPAddress bindAddress = IPAddress.Parse(_ConfigTree.Get("network.interface.ip", "0.0.0.0"));
            int bindPort = _ConfigTree.Get("network.interface.port", 5400);
            _Listener.Bind(bindAddress, bindPort);

            ConnectToAccountServer();

            //_RemotingServer = new RemotingServer(_ConfigTree.Get("inter.uri", "net.tcp://localhost:5501/"));
            //_RemotingServer.ExposeType<InterServerService>("InterServer", typeof(IInterServerService));
            //_RemotingServer.Start();
            //_Log.InfoFormat("Accepting CharServer connections at {0}", _RemotingServer.BaseUri);
            _Listener.Listen();
            _Log.InfoFormat("Accepting connections at {0}:{1}", bindAddress, bindPort);

            _Running = true;
            while (_Running)
                Thread.Yield();

            _Service.Proxy.Disconnect();

            _RemotingServer.Stop();
            _RemotingClient.Disconnect();
        }

        private void ConnectToAccountServer()
        {
            _RemotingClient = new RemotingClient(_ConfigTree.Get("inter.account", "net.tcp://localhost:4501/"));

            while (true)
            {
                _Log.InfoFormat("Connecting to AccountServer at {0}", _RemotingClient.BaseUri);

                try
                {
                    _Service = _RemotingClient.Get<ICharServerService>("CharServer");
                }
                catch (Exception ex)
                {
                    _Log.Error("Error connecting to AccountServer", ex);
                    Thread.Sleep(5000);
                    continue;
                }

                if (_Service != null)
                    break;
            }

            _Service.Proxy.RegisterServer("Rhea", _Listener.Address, _Listener.Port);
        }

        public AccountServerDataContext GetDataContext()
        {
            return new AccountServerDataContext(_DatabaseConnection);
        }

        NetworkConnectionBase INetworkConnectionFactory.Create(System.Net.Sockets.Socket client)
        {
            return new AccountServerClient(client);
        }

        private string BuildConnectionString()
        {
            return string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};",
                                 _ConfigTree.Get("database.server", "127.0.0.1"),
                                 _ConfigTree.Get("database.dbname", "rhea_logindb"),
                                 _ConfigTree.Get("database.username", "rhea"),
                                 _ConfigTree.Get("database.password", "rhea"));
        }
    }
}

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem;

public class Robot(string ipAddress = "localhost", int dashboardPort = 29999, int urscriptPort = 30002)
{
    private readonly TcpClient _dash = new();
    private readonly TcpClient _urs = new();
    private Stream _dashStream;
    private StreamReader _dashReader;
    private Stream _ursStream;

    public string IpAddress = ipAddress;
    public bool Connected => _dash.Connected && _urs.Connected;

    public void Connect()
    {
        _dash.Connect(IpAddress, dashboardPort);
        _dashStream = _dash.GetStream();
        _dashReader = new StreamReader(_dashStream, Encoding.ASCII);
        _ = _dashReader.ReadLine(); // konsumer velkomstlinjen

        _urs.Connect(IpAddress, urscriptPort);
        _ursStream = _urs.GetStream();
    }

    public string RobotMode
    {
        get { SendDashboard("robotmode\n"); return _dashReader.ReadLine(); }
    }

    public bool ProgramRunning
    {
        get { SendDashboard("running\n"); return _dashReader.ReadLine() == "Program running: true"; }
    }

    public async void PowerOn()
    {
        SendDashboard("power on\n");
        _ = _dashReader.ReadLine();
        while (RobotMode != "Robotmode: IDLE") await Task.Delay(500);
    }

    public async void BrakeRelease()
    {
        SendDashboard("brake release\n");
        _ = _dashReader.ReadLine();
        while (RobotMode != "Robotmode: RUNNING") await Task.Delay(500);
    }

    public void SendDashboard(string cmd) => _dashStream.Write(Encoding.ASCII.GetBytes(cmd));
    public void SendUrscript(string program) => _ursStream.Write(Encoding.ASCII.GetBytes(program));
    public void SendUrscriptFile(string path) => SendUrscript(File.ReadAllText(path) + Environment.NewLine);
    public void Disconnect() { _dash.Close(); _urs.Close(); }
}
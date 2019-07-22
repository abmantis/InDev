using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VenomNamespace
{ 
 class IPData
{
    private int numrecords;

    private DateTime uptimestart;
    private DateTime downtimestart;
    private bool isUp;

    private int _runningdropcount;

    bool mqttconnected;

    private string _ipaddress;
    public string IPAddress
    {
        get { return _ipaddress; }
        set { _ipaddress = value; }
    }

    private string _otapay;
    public string OTAPayload
        {
        get { return _otapay; }
        set { _otapay = value; }
    }

    private int _avg;
    public int Average
    {
        get { return _avg; }
    }

    private int _mqttdropcount;
    public int MQTTDropCount
    {
        get { return _mqttdropcount; }
    }

    public int MQTTUptimePct
    {
        get
        {
            try
            {
                return (int)((((decimal)numrecords - (decimal)_mqttdropcount) / (decimal)numrecords) * 100);
            }
            catch
            {
                return 0;
            }
        }
    }

    private int _dropcount;
    public int DropCount
    {
        get { return _dropcount; }
    }

    public int UptimePct
    {
        get
        {
            try
            {
                return (int)((((decimal)numrecords - (decimal)_dropcount) / (decimal)numrecords) * 100);
            }
            catch
            {
                return 0;
            }
        }
    }

    private TimeSpan _uptimelength;
    public TimeSpan ConnectedTime
    {
        get
        {
            _uptimelength = isUp ? DateTime.Now - uptimestart : new TimeSpan(0);
            return _uptimelength;
        }
    }

    private TimeSpan _downtimelength;
    public TimeSpan DisconnectedTime
    {
        get
        {
            _downtimelength = !isUp && _linkstate != -1 ? DateTime.Now - downtimestart : new TimeSpan(0);
            return _downtimelength;
        }
    }

    private TimeSpan _longestuptime;
    public TimeSpan LongestMQTTUptime
    {
        get { return _longestuptime; }
    }

    private int _linkstate;
    public int LinkState
    {
        get { return _linkstate; }
        set
        {
            if (value == 3)
            {
                if (_linkstate != 3)
                {
                    uptimestart = DateTime.Now;
                    _downtimelength = new TimeSpan(0);
                    isUp = true;
                }
            }
            else
            {
                if (_linkstate == 3 || _linkstate == -1)
                {
                    downtimestart = DateTime.Now;
                    _uptimelength = new TimeSpan(0);
                    isUp = false;
                    _mqttdropcount++;
                }
            }
            _linkstate = value;

        }
    }

    private int _claimstate;
    public int ClaimState
    {
        get { return _claimstate; }
        set { _claimstate = value; }
    }

    private string _wifiresyncstatistics;
    public string WifiResyncStatistics
    {
        get { return _wifiresyncstatistics; }
        set { _wifiresyncstatistics = value; }
    }

    public IPData(string ip, string pay)
    {
        _ipaddress = ip;
        _otapay = pay;
        numrecords = 0;
        _dropcount = 0;
        _runningdropcount = 0;
        _mqttdropcount = 0;
        _uptimelength = new TimeSpan(0);
        _downtimelength = new TimeSpan(0);
        _longestuptime = new TimeSpan(0);
        uptimestart = new DateTime(0);
        downtimestart = new DateTime(0);
        mqttconnected = false;
        _linkstate = -1;
        _claimstate = -1;
        isUp = false;
        _wifiresyncstatistics = "";
    }

    public void AddReply(int time)
    {
        numrecords++;
        _avg = ((numrecords - 1) * _avg + time) / numrecords;
        if (time == Venom.pingtimeout)
        {
            _dropcount++;
            _runningdropcount++;

            //Since can't read Link State from Trace if appliance loses connection, if it misses three pings in a row assume MQTT is also disconnected
            if (_runningdropcount >= 3)
            {
                this.LinkState = 0;
            }
            //uptimestart = uptimestart.Ticks == 0 ? DateTime.Now : uptimestart;
        }
        else
        {
            _runningdropcount = 0;
            /*if (uptimestart.Ticks != 0)
            {
                _uptimelength = DateTime.Now - uptimestart;
                uptimestart = new DateTime(0);
            }*/
        }

        /*if (mqttconn)
        {
            if (!mqttconnected)
            {
                uptimestart = DateTime.Now;
            }
            _uptimelength = DateTime.Now - uptimestart;
        }
        else
        {
            _mqttdropcount++;
            if (mqttconnected)
            {
                _longestuptime = new TimeSpan(Math.Max(_longestuptime.Ticks, _uptimelength.Ticks));
                _uptimelength = new TimeSpan(0);
            }
        }
        mqttconnected = mqttconn;*/

    }

}
}

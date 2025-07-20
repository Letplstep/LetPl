using UnityEngine;

//OSC ����� ���� HokuyoManage���κ��� ������ �޾ƿ��� ��ũ��Ʈ
public class OSCManager : MonoBehaviour
{
    public static OSCManager instance;

    public OSC _isOSC;

    public SensorDataFormat[] SensorData; //ȣ���κ��� ���� �����͸� ����

    //�̱���
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this) 
                Destroy(this.gameObject);
        }
    }

    //�ʱ�ȭ
    private void Start()
    {
        SetOSC_EventHandler();
        SensorData =  new SensorDataFormat[System.Enum.GetValues(typeof(SensorEnum)).Length];
        for (int i = 0; i < SensorData.Length; i++)
            SensorData[i] = new SensorDataFormat();
    }

    #region FrontSensor
    //���� ������� �ֽ�ȭ, ȣ��� �޴������� ������ �� ũ�� �� �ޱ�
    public void getFrontStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Front)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Front)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Front)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Front)] = true;
            Debug.Log("���� ���� ����");
        }
    }

    //�ν��� ��ü�� ��ġ�� �ޱ�
    public void getFrontSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Front)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
    }

    public void getFrontStopMessage(OscMessage message)
    {
    }

    //���� ���� ��ȣ �ޱ�
    public void FrontSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Front)] = false;
        Debug.Log("���� ���� ����");
    }
    #endregion

    #region BackSensor
    public void getBackStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Back)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Back)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Back)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Back)] = true;
            Debug.Log("�ĸ� ���� ����");
        }
    }

    public void getBackSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Back)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
        //Debug.Log(SensorData.Position.Count);
    }

    public void getBackStopMessage(OscMessage message)
    {
        //Debug.Log(message.GetInt(0));
    }

    public void BackSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Back)] = false;
        Debug.Log("�ĸ� ���� ����");
    }
    #endregion

    #region RightSensor
    public void getRightStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Right)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Right)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Right)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Right)] = true;
            Debug.Log("��� ���� ����");
        }
    }

    public void getRightSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Right)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
        //Debug.Log(SensorData.Position.Count);
    }

    public void getRightStopMessage(OscMessage message)
    {
        //Debug.Log(message.GetInt(0));
    }

    public void RightSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Right)] = false;
        Debug.Log("��� ���� ����");
    }
    #endregion

    #region LeftSensor
    public void getLeftStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Left)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Left)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Left)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Left)] = true;
            Debug.Log("�¸� ���� ����");
        }
    }

    public void getLeftSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Left)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
        //Debug.Log(SensorData.Position.Count);
    }

    public void getLeftStopMessage(OscMessage message)
    {
        //Debug.Log(message.GetInt(0));
    }

    public void LeftSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Left)] = false;
        Debug.Log("�¸� ���� ����");
    }
    #endregion

    #region DownSensor
    public void getDownStartMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Down)].RectSize = new Vector2(message.GetFloat(0), message.GetFloat(1));
        SensorData[((int)SensorEnum.Down)].Position.Clear();

        if (!SensorActiveState.instance.SensorState[((int)SensorEnum.Down)])
        {
            SensorActiveState.instance.SensorState[((int)SensorEnum.Down)] = true;
            Debug.Log("�ٴ� ���� ����");
        }
    }

    public void getDownSensorMessage(OscMessage message)
    {
        SensorData[((int)SensorEnum.Down)].Position.Add(new Vector3(message.GetFloat(0), message.GetFloat(1), 0));
    }

    public void getDownStopMessage(OscMessage message)
    {
    }

    public void DownSensorQuit(OscMessage message)
    {
        SensorActiveState.instance.SensorState[((int)SensorEnum.Down)] = false;
        Debug.Log("�ٴ� ���� ����");
    }
    #endregion

    void SetOSC_EventHandler()
    {
        _isOSC.SetAddressHandler("/Front/Start", getFrontStartMessage);
        _isOSC.SetAddressHandler("/Front/Data", getFrontSensorMessage);
        _isOSC.SetAddressHandler("/Front/End", getFrontStopMessage);
        _isOSC.SetAddressHandler("/Front/Quit", FrontSensorQuit);
        _isOSC.SetAddressHandler("/Back/Start", getBackStartMessage);
        _isOSC.SetAddressHandler("/Back/Data", getBackSensorMessage);
        _isOSC.SetAddressHandler("/Back/End", getBackStopMessage);
        _isOSC.SetAddressHandler("/Back/Quit", BackSensorQuit);
        _isOSC.SetAddressHandler("/Left/Start", getLeftStartMessage);
        _isOSC.SetAddressHandler("/Left/Data", getLeftSensorMessage);
        _isOSC.SetAddressHandler("/Left/End", getLeftStopMessage);
        _isOSC.SetAddressHandler("/Left/Quit", LeftSensorQuit);
        _isOSC.SetAddressHandler("/Right/Start", getRightStartMessage);
        _isOSC.SetAddressHandler("/Right/Data", getRightSensorMessage);
        _isOSC.SetAddressHandler("/Right/End", getRightStopMessage);
        _isOSC.SetAddressHandler("/Right/Quit", RightSensorQuit);
        _isOSC.SetAddressHandler("/Down/Start", getDownStartMessage);
        _isOSC.SetAddressHandler("/Down/Data", getDownSensorMessage);
        _isOSC.SetAddressHandler("/Down/End", getDownStopMessage);
        _isOSC.SetAddressHandler("/Down/Quit", DownSensorQuit);
    }
}

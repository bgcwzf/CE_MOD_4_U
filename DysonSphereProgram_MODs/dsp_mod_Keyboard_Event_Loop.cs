using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices; //for NoInlining
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
//using System.Speech.Synthesis;
//ע�⣺���а����������ַ���cs�ļ�����Ҫ��GB2312���뷽ʽ����
//���漸�� //MOD_XXXX==> �ǹ̶�д�������ڸ�CE�ṩ��ؼ�����Ϣ�����밴�˸�ʽд�����Patch��֮�䣬��Ӣ�Ķ��ŷָ�
//MOD_PATCH_TARGET==>GameData:Update
//MOD_NEW_METHOD==>DSP_CE_MOD.GameData_Update_Patch:new_Update
//MOD_OLD_CALLER==>DSP_CE_MOD.GameData_Update_Patch:old_Update
//MOD_DESCRIPTION==>���ڽ��պʹ�������Ϸ�еİ�����Ϣ��ͨ��ʹ��F1~F10��ִ���ض��Ĺ���
namespace DSP_CE_MOD
{
    /*
     ��������MODģ���У������Ҫ�Լ����¼�������Ӧ�������ͨ�����´�����ʵ�֣�
    �ڵ������Լ������У�ʵ��ί�з�����
    // �����¼���ί����Ӧ����
    private void onKeyResp(KeyCode keyCode)
    {
        // ����ʵ����ϣ���ڰ����¼�����ʱִ�е��߼�
    }
    // Ȼ�����
    KeyboardEventManager.Instance.RegisterKeyEventHandler(onKeyResp);
     */
    
    public class KeyboardEventManager
    {
        // ˽�о�̬�������洢���Ψһʵ��
        //private static readonly KeyboardEventManager instance = new KeyboardEventManager();

        // ˽�й��캯������ֹ�ⲿʵ����
        //private KeyboardEventManager() { }

        // ������̬�����ṩΨһʵ���ķ���
        /*public static KeyboardEventManager Instance
        {
            get
            {
                Print_Message.Print(string.Format("Keyboard Logic Get Instance2"));
                return instance;
            }
        }
        */

        // ʹ��System.Action�����Զ���ί��
        private event Action<KeyCode> keyEvent;

        // �ṩһ�������ķ��������ⲿ������ע��ί����
        public void RegisterKeyEventHandler(Action<KeyCode> handler)
        {
            keyEvent += handler;
            /*
            if (keyEvent != null)
                Print_Message.Print(string.Format("ע��ǰ��keyEvent ������Ч��2"));
            else
                Print_Message.Print(string.Format("ע��ǰ��keyEvent ����ΪNULL2"));
            keyEvent += handler;
            if (keyEvent != null)
                Print_Message.Print(string.Format("ע���keyEvent ������Ч��2"));
            else
                Print_Message.Print(string.Format("ע���keyEvent ����ΪNULL2"));*/
        }

        // �ṩһ�������ķ������ڽ��ע��ί����
        public void UnregisterKeyEventHandler(Action<KeyCode> handler)
        {
            keyEvent -= handler;
        }

        // �ṩһ�������ķ�������ɾ��������ע���ί����
        public void ClearKeyEventHandler()
        {
            keyEvent = null;
        }

        public KeyCode GetKeyDownCode()
        {//���� Ctrl+F10 ʱ��������еļ����¼��������
            if (Input.anyKeyDown)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        Debug.Log(key.ToString());
                        if (key == KeyCode.F10&& (Input.GetKey(KeyCode.LeftControl) ||Input.GetKey(KeyCode.RightControl)))
                        {//���� Ctrl+F10 ʱ
                            ClearKeyEventHandler();
                        }
                        return key;
                    }
                }
            }
            return KeyCode.None;
        }

        public void InvokeKeyEvent()
        {
            if (keyEvent != null)
                keyEvent.Invoke(GetKeyDownCode());
            else
                Print_Message.Print(string.Format("keyEvent ����ΪNULL"));
        }
    }

    public static class KeyEvtMgr
    {
        public static KeyboardEventManager key_evt = null;
        static KeyEvtMgr()
        {//��̬���캯����ִֻ��һ�Σ���������������ȫ�����ݴ洢�������ݲ��ܷ��ڱ�Patch�������桿
            key_evt = new KeyboardEventManager();
            Print_Message.Print(string.Format("------------------KeyEvtMgr Init Complete.-------------------------"));
        }
        public static void ReleaseObj()
        {
            key_evt = null;
        }
    }

    public class GameData_Update_Patch : GameData
    {

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_Update()
        {
            old_Update();//�ȵ���ԭ����Update����
            try
            {
                if (KeyEvtMgr.key_evt != null)
                {
                    if (Input.anyKeyDown)
                    {
                        Print_Message.Print(string.Format("Keyboard Logic �а�������"));
                        KeyEvtMgr.key_evt.InvokeKeyEvent();
                    }
                }
                else
                {
                    //Print_Message.Print(string.Format("KeyEvtMgr.key_evt is NULL .........................now try to create it"));
                    KeyEvtMgr.key_evt.GetKeyDownCode();
                }
                //var gamedata = GameMain.data;
                //var player = gamedata.mainPlayer;
            }
            catch (Exception ex)
            {
                Print_Message.Print(string.Format("Keyboard Logic�������߼�ʱ�����쳣: {0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_Update() { }
    }
}
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
//注意：所有包含了中文字符的cs文件，需要以GB2312编码方式保存
//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。多个Patch项之间，用英文逗号分隔
//MOD_PATCH_TARGET==>GameData:Update
//MOD_NEW_METHOD==>DSP_CE_MOD.GameData_Update_Patch:new_Update
//MOD_OLD_CALLER==>DSP_CE_MOD.GameData_Update_Patch:old_Update
//MOD_DESCRIPTION==>用于接收和处理在游戏中的按键信息。通常使用F1~F10来执行特定的功能
namespace DSP_CE_MOD
{
    /*
     在其他的MOD模块中，如果需要对键盘事件进行响应，则可以通过以下代码来实现：
    在调用者自己的类中，实现委托方法：
    // 按键事件的委托响应方法
    private void onKeyResp(KeyCode keyCode)
    {
        // 这里实现您希望在按键事件发生时执行的逻辑
    }
    // 然后调用
    KeyboardEventManager.Instance.RegisterKeyEventHandler(onKeyResp);
     */
    
    public class KeyboardEventManager
    {
        // 私有静态变量，存储类的唯一实例
        //private static readonly KeyboardEventManager instance = new KeyboardEventManager();

        // 私有构造函数，防止外部实例化
        //private KeyboardEventManager() { }

        // 公开静态方法提供唯一实例的访问
        /*public static KeyboardEventManager Instance
        {
            get
            {
                Print_Message.Print(string.Format("Keyboard Logic Get Instance2"));
                return instance;
            }
        }
        */

        // 使用System.Action代替自定义委托
        private event Action<KeyCode> keyEvent;

        // 提供一个公开的方法用于外部调用者注册委托项
        public void RegisterKeyEventHandler(Action<KeyCode> handler)
        {
            keyEvent += handler;
            /*
            if (keyEvent != null)
                Print_Message.Print(string.Format("注册前，keyEvent 对象有效！2"));
            else
                Print_Message.Print(string.Format("注册前，keyEvent 对象为NULL2"));
            keyEvent += handler;
            if (keyEvent != null)
                Print_Message.Print(string.Format("注册后，keyEvent 对象有效！2"));
            else
                Print_Message.Print(string.Format("注册后，keyEvent 对象为NULL2"));*/
        }

        // 提供一个公开的方法用于解除注册委托项
        public void UnregisterKeyEventHandler(Action<KeyCode> handler)
        {
            keyEvent -= handler;
        }

        // 提供一个公开的方法用于删除所有已注册的委托项
        public void ClearKeyEventHandler()
        {
            keyEvent = null;
        }

        public KeyCode GetKeyDownCode()
        {//按下 Ctrl+F10 时，清除所有的键盘事件处理程序
            if (Input.anyKeyDown)
            {
                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        Debug.Log(key.ToString());
                        if (key == KeyCode.F10&& (Input.GetKey(KeyCode.LeftControl) ||Input.GetKey(KeyCode.RightControl)))
                        {//按下 Ctrl+F10 时
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
                Print_Message.Print(string.Format("keyEvent 对象为NULL"));
        }
    }

    public static class KeyEvtMgr
    {
        public static KeyboardEventManager key_evt = null;
        static KeyEvtMgr()
        {//静态构造函数，只执行一次，正好用它来构造全局数据存储区【数据不能放在被Patch的类里面】
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
            old_Update();//先调用原来的Update函数
            try
            {
                if (KeyEvtMgr.key_evt != null)
                {
                    if (Input.anyKeyDown)
                    {
                        Print_Message.Print(string.Format("Keyboard Logic 有按键按下"));
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
                Print_Message.Print(string.Format("Keyboard Logic处理按键逻辑时发生异常: {0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_Update() { }
    }
}
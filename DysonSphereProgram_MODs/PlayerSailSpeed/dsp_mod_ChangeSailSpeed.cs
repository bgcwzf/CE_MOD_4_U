using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

//MOD_PATCH_TARGET==>UTIL
//MOD_NEW_METHOD==>
//MOD_OLD_CALLER==>
//MOD_DESCRIPTION==>工具类，本模块是特殊的模块，通过键盘热键来执行相应的操作，不执行Patch操作。
namespace DSP_CE_MOD
{
    public class PlayerSailSeed_Patch : GameData_Update_Patch
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PopupMessage(string msg,bool sound=false)
        {
            UIRealtimeTip.Popup(msg, sound, 0);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ShowMessageBox(string msg)
        {
            //UIMessageBox.Show("title",msg,"btn1_name","btn2_name", 2, null, new UIMessageBox.Response(this.onBtnClick));
            //var box = UIMessageBox.Show("拆除储物仓标题".Translate(), "拆除储物仓文字".Translate(), "否".Translate(), "是".Translate(), 1, new UIMessageBox.Response(this.DismantleQueryCancel), new UIMessageBox.Response(this.DismantleQueryConfirm));
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
		public void SpeedUp()
        {
            const float c_ratio = 1000.0f;// 常数1000是游戏内部的速度固有系数
            var player = GameMain.data.mainPlayer;
			var real_global_speed = player.uVelocity.magnitude;
            var now_ratio = real_global_speed / c_ratio;
            double speed_change_ratio = now_ratio * (int)DSP_Storage_Vault.ce_mod_data_exchange[3];
            if (speed_change_ratio < 0.000001)
            {
                player.uVelocity.x =
                player.uVelocity.y =
                player.uVelocity.z = 0.0001;
                return;
            }
            var vx = player.uVelocity.x / real_global_speed;
            var vy = player.uVelocity.y / real_global_speed;
            var vz = player.uVelocity.z / real_global_speed;

			player.uVelocity.x= c_ratio * vx * speed_change_ratio;
            player.uVelocity.y= c_ratio * vy * speed_change_ratio;
            player.uVelocity.z= c_ratio * vz * speed_change_ratio;
            //PopupMessage(string.Format("速度从{0}变为{1},新速度是旧速度的{2}倍", real_global_speed, player.uVelocity.magnitude, player.uVelocity.magnitude/real_global_speed));
            /*

            Print_Message.Print(string.Format("org vx:{0}, after:{1}",vx, c_ratio * vx * speed_change_ratio));
            Print_Message.Print(string.Format("org vy:{0}, after:{1}", vy, c_ratio * vy * speed_change_ratio));
            Print_Message.Print(string.Format("org vz:{0}, after:{1}", vy, c_ratio * vz * speed_change_ratio));
local x,y,z
x=readDouble(u_speed)
y=readDouble(u_speed+8)
z=readDouble(u_speed+8*2)

--[[ 因为目的是让xyz三个分速度的合速度增大指定的倍数，并保持速度的方向不变
直接将原始速度分量乘以times（即new_x = times * x）会改变速度向量的方向，
因为这样仅仅是将每个分量按相同的比例增加，而不是保持它们之间的相对比例关系。
当原始速度向量不是沿着一个主轴时，这将导致速度向量方向的改变。
因此，为了保持原始速度方向不变，必须首先将速度向量标准化，然后按比例增加其大小。
]]--
local real_global_speed=math.sqrt(x*x+y*y+z*z)
-- 计算速度向量每个分量相对于其总长度的比例（标准化）
x=x/real_global_speed
y=y/real_global_speed
z=z/real_global_speed
--print(u_speed,":",sign,":",times,":end")
-- 常数1000是游戏内部的速度固有系数
writeDouble(u_speed,1000*times*x)
--writeDouble(player_uspeed_times_addr+8,1000*times*x)

u_speed=u_speed+8
writeDouble(u_speed,1000*times*y)
--writeDouble(player_uspeed_times_addr+8*2,1000*times*y)

u_speed=u_speed+8
writeDouble(u_speed,1000*times*z)
--writeDouble(player_uspeed_times_addr+8*3,1000*times*z)
*/
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SpeedDown()
        {
            const float c_ratio = 1000.0f;// 常数1000是游戏内部的速度固有系数
            var player = GameMain.data.mainPlayer;
            var real_global_speed = player.uVelocity.magnitude;
            var now_ratio = real_global_speed / c_ratio;
            double speed_change_ratio = now_ratio / (int)DSP_Storage_Vault.ce_mod_data_exchange[3];
            if (speed_change_ratio < 0.000001)
            {
                player.uVelocity.x =
                player.uVelocity.y =
                player.uVelocity.z = 0.0001;
                return;
            }
            var vx = player.uVelocity.x / real_global_speed;
            var vy = player.uVelocity.y / real_global_speed;
            var vz = player.uVelocity.z / real_global_speed;
            player.uVelocity.x = c_ratio * vx * speed_change_ratio;
            player.uVelocity.y = c_ratio * vy * speed_change_ratio;
            player.uVelocity.z = c_ratio * vz * speed_change_ratio;
            //PopupMessage(string.Format("速度从{0}变为{1},新速度是旧速度的{2}倍", real_global_speed, player.uVelocity.magnitude, player.uVelocity.magnitude / real_global_speed));
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void SpeedToZero()
        {
            var player = GameMain.data.mainPlayer;
			player.uVelocity.x =
			player.uVelocity.y =
			player.uVelocity.z = 0;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void onKeyResp(KeyCode keyCode)
        {
            Print_Message.Print(string.Format("PlayerSailSeed_Patch ...onKeyResp(KeyCode:{0})", keyCode));
            var pv_planetId = PrivateHelper<Player>.GetPrivateField("_planetId");
            var player = GameMain.data.mainPlayer;
            if (player.movementState!= EMovementState.Sail)//当不在太空中航行时，这些快捷键不生效。但是可以执行卸载程序
            {// || (int)pv_planetId.GetValue(player) != 0
                //unregist_key_event();
                //PopupMessage("因为现在已不在太空航行状态中，所以航行速度操作MOD暂时无效。");
                return;
            }
            //string tmp_str = "";
            //int tmp_val = 0;
            if (DSP_Storage_Vault.ce_mod_data_exchange[3] == null) //这个静态全局表中的第三项，用于存放改变航行速度的倍数因子的值
            {
                //因为第一次使用时，该项为null，首先要装箱一个int对象进来，然后CE中才可以修改它的值
                int a = 10;
                DSP_Storage_Vault.ce_mod_data_exchange[3] = (object)a;
            }
            switch (keyCode)
            {
                case KeyCode.F1:
                    if (Input.GetKey(KeyCode.LeftControl))
                    {//按下左边的Ctrl+F1的时候，就是卸载当前键盘事件处理的代理程序
                        unregist_key_event();
                        PopupMessage("已临时禁用机甲太空航行速度调整MOD，需要在CE中，重新勾选相应的条目进行启用");
                        break;
                    }
					//以下是只按下F1键的处理
					//正式开始执行矿脉重排的任务
					SpeedUp();
					VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                    PopupMessage(string.Format("已经加速为原来速度的{0}倍",(int)DSP_Storage_Vault.ce_mod_data_exchange[3]));
                    break;
                case KeyCode.F2:
					SpeedDown();
					PopupMessage(string.Format("已经减速为原来速度的{0}分之1", (int)DSP_Storage_Vault.ce_mod_data_exchange[3]));
                    VFAudio.Create("equip-0", null, Vector3.zero, true);
                    break;
                case KeyCode.F3:
					SpeedToZero();
					PopupMessage("航行速度设置为0");
                    VFAudio.Create("zscreen-exit", null, Vector3.zero, true);
                    break;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void regist_key_event()
        {
            VFAudio.Create("ui-click-2", null, Vector3.zero, true);
            Print_Message.Print(string.Format("Hot key Enable State...{0}", is_hot_key_set));
            if (is_hot_key_set == 0)
            {
                KeyEvtMgr.key_evt.RegisterKeyEventHandler(onKeyResp);
                Print_Message.Print(string.Format("[SUCCESS]Enable Hot key for this mod...."));
                is_hot_key_set = 1;
                GameMain.onGameEnded += this.OnGameEnded;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void unregist_key_event()
        {
            GameMain.onGameEnded -= this.OnGameEnded;
            VFAudio.Create("ui-error", null, Vector3.zero, true);
            KeyEvtMgr.key_evt.UnregisterKeyEventHandler(onKeyResp);
            is_hot_key_set = 0;
            Print_Message.Print(string.Format("KeyEvtMgr.key_evt.UnregisterKeyEventHandler(\"PlayerSailSeed_Patch\") ... unloaded...."));
        }
        private static int is_hot_key_set = 0;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void OnGameEnded()
        {
            KeyEvtMgr.key_evt.UnregisterKeyEventHandler(onKeyResp);
            is_hot_key_set = 0;
            Print_Message.Print(string.Format("KeyEvtMgr.key_evt.UnregisterKeyEventHandler(\"PlayerSailSeed_Patch\") ... unloaded...."));
        }
    }
}
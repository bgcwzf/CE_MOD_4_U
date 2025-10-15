using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;


//以下是旧的注释
//MOD_PATCH_TARGET==>PlayerAction_Inspect:SetInspectee
//MOD_NEW_METHOD==>DSP_CE_MOD.RegouopVeins_PlayerAction_Inspect:new_SetInspectee
//MOD_OLD_CALLER==>DSP_CE_MOD.RegouopVeins_PlayerAction_Inspect:old_SetInspectee
//MOD_DESCRIPTION==>a MONO MOD DLL
namespace DSP_CE_MOD
{
    public class RegouopVeins_PlayerAction_Inspect : PlayerAction_Inspect
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void PopupMessage(string msg,bool sound=false)
        {
            UIRealtimeTip.Popup(msg, sound, 0);
            //UIRealtimeTip.PopupAhead();
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ShowMessageBox(string msg)
        {
            //UIMessageBox.Show("title",msg,"btn1_name","btn2_name", 2, null, new UIMessageBox.Response(this.onBtnClick));
            //var box = UIMessageBox.Show("拆除储物仓标题".Translate(), "拆除储物仓文字".Translate(), "否".Translate(), "是".Translate(), 1, new UIMessageBox.Response(this.DismantleQueryCancel), new UIMessageBox.Response(this.DismantleQueryConfirm));
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
            Print_Message.Print(string.Format("KeyEvtMgr.key_evt.UnregisterKeyEventHandler(\"ResourceVeinsManager_Patch\") ... unloaded...."));
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void OnGameEnded()
        {
            KeyEvtMgr.key_evt.UnregisterKeyEventHandler(onKeyResp);
            is_hot_key_set = 0;
            Print_Message.Print(string.Format("KeyEvtMgr.key_evt.UnregisterKeyEventHandler(\"ResourceVeinsManager_Patch\") ... unloaded...."));
        }
   
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void onKeyResp(KeyCode keyCode)
        {
            Print_Message.Print(string.Format("RegouopVeins_PlayerAction_Inspect ...onKeyResp(KeyCode:{0})",keyCode));
            var pv_planetId = PrivateHelper<Player>.GetPrivateField("_planetId");
            int cur_planet_id = (int)pv_planetId.GetValue(this.player);
            if (DSP_Storage_Vault.ce_mod_data_exchange[4] == null)
            {
                DSP_Storage_Vault.ce_mod_data_exchange[4] = (object)current_group_idx;
            }
            current_group_idx = (int)DSP_Storage_Vault.ce_mod_data_exchange[4];//从全局共享数据区的第四个位置取出来当前序号值
            Print_Message.Print(string.Format("RegouopVeins_PlayerAction_Inspect.Current Resource Group ID: {0} on Planet [ID:{1}]", current_group_idx, cur_planet_id));
            if (this.player.movementState == EMovementState.Sail|| cur_planet_id <= 0)//在太空中航行时，这些快捷键不生效。但是可以执行卸载程序
            {
                //unregist_key_event();
                //PopupMessage("因为现在已离开星球，所以先禁用资源组操作MOD。");
                last_planet_id = 0;
                current_group_idx = 0;
                //保存新值到全局共享数据区的第四个位置
                DSP_Storage_Vault.ce_mod_data_exchange[4] = (object)current_group_idx;
                DSP_Storage_Vault.ce_mod_data_exchange[2] = (object)current_group_idx;//类型也置0
                return;
            }
            if (last_planet_id != cur_planet_id)
            {//行星发生了变化，也要重置当前序号
                current_group_idx = 0;
                DSP_Storage_Vault.ce_mod_data_exchange[4] = (object)current_group_idx;
                DSP_Storage_Vault.ce_mod_data_exchange[2] = (object)current_group_idx;//类型也置0
            }
            last_planet_id = cur_planet_id;
            string tmp_str = "";
            int tmp_val=0;
            if (DSP_Storage_Vault.ce_mod_data_exchange[2] == null)
            {
                //因为第一次使用时，该项为null，首先要装箱一个int对象进来，然后CE中才可以修改它的值
                int a = 0;
                DSP_Storage_Vault.ce_mod_data_exchange[2] = (object)a;
            }
            EVeinType veinType=EVeinType.None;
            switch (keyCode)
            {
                case KeyCode.F1:
                    if (Input.GetKey(KeyCode.LeftControl))
                    {//按下左边的Ctrl+F1的时候，就是卸载当前键盘事件处理的代理程序
                        unregist_key_event();
                        PopupMessage("已临时禁用资源处理MOD，需要点选某个矿脉对象进行启用");
                        break;
                    }
                    if (Input.GetKey(KeyCode.LeftShift))
                    {//按下左边的Shift+F1的时候，初始化当前的groupIdx=1
                        current_group_idx = 1;
                        DSP_Storage_Vault.ce_mod_data_exchange[4] = (object)current_group_idx;
                        DSP_Storage_Vault.ce_mod_data_exchange[2] = (object)((int)this.player.planetData.factory.veinGroups[current_group_idx].type);
                        PopupMessage("已经将当前资源组的操作序号设置为 1");
                        //因为【0】号元素是默认的一个占位元素。所以有效的下标从1开始
                        break;
                    }
                    //以下是只按下F1键的处理
                    //正式开始执行矿脉重排的任务
                    ArrangeVeins();
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);//, 5, -1, -1L);
                    PopupMessage("已经原地集合了当前星球上的所有矿脉【需要刷新后查看】");
                    break;
                case KeyCode.F2:
                    if (Input.GetKey(KeyCode.LeftShift))
                    {//按下左边的Shift+F2的时候，每按键一次，所选定的资源组序号【减】1
                        if (current_group_idx - 1 <= 0)
                        {
                            //VFAudio.Create("ui-error", null, Vector3.zero, true);
                            PopupMessage("资源组序号已经是 1 了。无法再减小。", true);
                            //因为【0】号元素是默认的一个占位元素。所以有效的下标从1开始
                            break;
                        }
                        current_group_idx--;
                        DSP_Storage_Vault.ce_mod_data_exchange[4] = (object)current_group_idx;
                        DSP_Storage_Vault.ce_mod_data_exchange[2] = (object)((int)this.player.planetData.factory.veinGroups[current_group_idx].type);
                        PopupMessage(string.Format("当前资源组序号：{0}", current_group_idx));
                        break;
                    }
                    //按下F2的时候，每按键一次，所选定的资源组序号【加】1
                    if (current_group_idx + 1 >= this.player.planetData.factory.veinGroups.Length)
                    {
                        //VFAudio.Create("ui-error", null, Vector3.zero, true);
                        PopupMessage(string.Format("资源组序号已经是最大值 {0} 了。无法再增加。", current_group_idx), true);
                        break;
                    }
                    current_group_idx++;
                    DSP_Storage_Vault.ce_mod_data_exchange[4] = (object)current_group_idx;
                    DSP_Storage_Vault.ce_mod_data_exchange[2] = (object)((int)this.player.planetData.factory.veinGroups[current_group_idx].type);
                    PopupMessage(string.Format("当前资源组序号：{0}", current_group_idx));
                    break;
                case KeyCode.F3:
                    if (current_group_idx <= 0)
                    {
                        PopupMessage(string.Format("操作失败。无效的资源组序号。"), true);
                        break;
                    }
                    //改变当前选定的“资源组序号”对应的矿脉的类型。具体类型id由DSP_Storage_Vault::ce_mod_data_exchange这个全局静态数组的下标[2]来指定(可以在CE中动态指定)
                    tmp_val = (int)DSP_Storage_Vault.ce_mod_data_exchange[2];
                    if (tmp_val <= 0 || tmp_val >= (int)EVeinType.Max)
                    {
                        PopupMessage(string.Format("改变当前选定的资源组[{0}]的类型值为[{1}]的操作失败。请先去设置目标矿脉资源为有效值。", current_group_idx, tmp_val), true);
                        break;
                    }
                    else
                    {
                        veinType = (EVeinType)(tmp_val);
                        tmp_str = string.Format("改变当前选定的资源组[{0}]的类型为[{1}]", current_group_idx, veinType );
                    }
                    if (SetTypeOfResourceGroup(current_group_idx))
                    {
                        VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                        PopupMessage(string.Format("【成功】{0}", tmp_str));
                    }
                    else
                        PopupMessage(string.Format("【失败】{0}", tmp_str), true);
                    break;
                case KeyCode.F4:
                    //在机甲当前所在的位置，创建上述类型的全新矿脉【创建之后需要刷新】
                    //创建所需的矿脉的类型。具体类型id由DSP_Storage_Vault::ce_mod_data_exchange这个全局静态数组的下标[2]来指定(可以在CE中动态指定)
                    tmp_val = (int)DSP_Storage_Vault.ce_mod_data_exchange[2];
                    if (tmp_val <= 0 || tmp_val >= (int)EVeinType.Max)
                    {
                        PopupMessage(string.Format("创建新的类型值为[{0}]的资源组的操作失败。请先去设置目标矿脉资源为有效值。", tmp_val), true);
                        break;
                    }
                    else
                    {
                        veinType = (EVeinType)(tmp_val);
                        tmp_str = string.Format("创建新的类型值为[{0}]的矿脉资源组的操作", veinType);
                    }
                    if (AddResourceGroup())
                    {
                        VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                        PopupMessage(string.Format("【成功】{0}【需要刷新后使用】", tmp_str));
                    }
                    else
                        PopupMessage(string.Format("【失败】{0}", tmp_str), true);
                    break;
                case KeyCode.F5:
                    //将当前选定资源组，移动到机甲所在的当前位置
                    if (current_group_idx <= 0)
                    {
                        PopupMessage(string.Format("操作失败。无效的资源组序号。"), true);
                        break;
                    }
                    tmp_str = string.Format("移动当前选定的资源组[{0}]到机甲当前所在位置", current_group_idx);
                    if (MoveResourceGroupToPos(current_group_idx, this.player.position))
                    {
                        VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                        PopupMessage(string.Format("【成功】{0}【需要刷新后使用】", tmp_str));
                    }
                    else
                        PopupMessage(string.Format("【失败】{0}", tmp_str), true);
                    break;
                case KeyCode.F6:
                    //将当前选定资源组的矿产储量翻倍
                    if (current_group_idx <= 0)
                    {
                        PopupMessage(string.Format("操作失败。无效的资源组序号。"), true);
                        break;
                    }
                    tmp_str = string.Format("大幅增加当前选定的资源组[{0}]的资源含量", current_group_idx);
                    if (SetAmoutOfResourceGroup(current_group_idx))
                    {
                        VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                        PopupMessage(string.Format("【成功】{0}", tmp_str));
                    }
                    else
                        PopupMessage(string.Format("【失败】{0}", tmp_str), true);
                    break;
                case KeyCode.F7:
                    //在当前选定资源组中，添加8个同类型的矿脉子项
                    if (current_group_idx <= 0)
                    {
                        PopupMessage(string.Format("操作失败。无效的资源组序号。"), true);
                        break;
                    }
                    tmp_str = string.Format("为当前选定的资源组[{0}]添加8个同类型的矿脉子项", current_group_idx);
                    if (AddVeinToResourceGroup(current_group_idx, 8))
                    {
                        VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                        PopupMessage(string.Format("【成功】{0}", tmp_str));
                    }
                    else
                        PopupMessage(string.Format("【失败】{0}", tmp_str), true);
                    break;
                case KeyCode.F8:
                    //this.player.factory.DebugEntityGUI();
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                    this.player.planetData.physics.SetPlanetPhysicsColliderDirty();
                    this.player.planetData.physics.RefreshColliders();
                    //GameMain.history.solarSailLife += 10000000;
                    break;
                case KeyCode.F9:
                    // VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                    //GameMain.history.solarSailLife += 100;
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                    //GameMain.history.solarSailLife = 10000000;
                    break;
                case KeyCode.F10:
                    break;
            }
        }
        public void ArrangeVeins()
        {
            Vector3[] vecOfEachGroup = new Vector3[100];
            bool[] setSign = new bool[100];

            var pool = this.player.planetData.factory.veinPool;
            var grps = this.player.planetData.factory.veinGroups;

            /* 
            Vector3[] vecOfEachGroup = new Vector3[100];
            // 注意，这里有一个巨大的“天坑”！！！！Vector3是一个Struct类型，对它的访问和赋值，都是“值访问”而不是“引用”
            //将对象数组中的某项取出来之后，得到的是对象的副本，对其进行修改，是无法影响原来的对象的。这一点与Python和JavaScript不同！！！！
            Vector3 p =vecOfEachGroup[0];
            p.x = 100;
            Print_Message.Print(string.Format("1.vecOfEachGroup[0].x={0}", vecOfEachGroup[0].x)); //这里居然会得到0，应该是100才对！！
            vecOfEachGroup[0].x = 200;
            Print_Message.Print(string.Format("2.vecOfEachGroup[0].x={0}", vecOfEachGroup[0].x));
             */
            for (int i=0;i<pool.Length;i++) {
                var v = pool[i];
                //Print_Message.Print(string.Format("This Vein=>id:{0},group index:{1},amount:{2},pos:({3},{4},{5})", v.id,v.groupIndex,v.amount,v.pos.x,v.pos.y,v.pos.z));
                if (v.id <= 0 || v.groupIndex <= 0 || v.amount <= 0 || (grps[v.groupIndex].type == EVeinType.None|| grps[v.groupIndex].type == EVeinType.Oil) || grps[v.groupIndex].count <= 0) continue;

                var p = vecOfEachGroup[v.groupIndex];
                //Print_Message.Print(string.Format("  Before set, the tmp pos array[{0}]:({1},{2},{3})", v.groupIndex, p.x, p.y, p.z));
                if (setSign[v.groupIndex]!=true)
                {
                    setSign[v.groupIndex] = true;
                    vecOfEachGroup[v.groupIndex].x = v.pos.x;
                    vecOfEachGroup[v.groupIndex].y = v.pos.y;
                    vecOfEachGroup[v.groupIndex].z = v.pos.z;
                }
                else
                {
                    pool[i].pos.x = p.x;
                    pool[i].pos.y = p.y;
                    pool[i].pos.z = p.z;
                }
                //Print_Message.Print(string.Format("  After set, the vein[{0}] pos :({1},{2},{3})", i, v.pos.x, v.pos.y, v.pos.z));
            }
            for (int i = 0; i < pool.Length; i++)
            {
                var v = pool[i];
                if (v.id <= 0 || v.groupIndex <= 0 || v.amount <= 0 || (grps[v.groupIndex].type == EVeinType.None || grps[v.groupIndex].type == EVeinType.Oil) || grps[v.groupIndex].count <= 0) continue;
                Print_Message.Print(string.Format("[ALL_DONE]This Vein=>id:{0},type={6},group index:{1},amount:{2},pos:({3},{4},{5})", v.id, v.groupIndex, v.amount, v.pos.x, v.pos.y, v.pos.z,v.type));
            }
            for (int i = 0; i < grps.Length; i++)
            {
                var v = grps[i];
                if (v.isNull || v.amount <= 0 || (v.type == EVeinType.None )) continue;
                Print_Message.Print(string.Format(">>> Vein GROUPs on this planet=>idx:{0},type={1},amount:{2},vein count in group:{6}, pos:({3},{4},{5})", i, v.type, v.amount, v.pos.x, v.pos.y, v.pos.z,v.count));
            }
            after_work_of_veins();
        }

        public void SetObjectPos(EObjectType objType, int objid, Vector3 new_pos)
        {
            if (objid <= 0)
            {
                return ;
            }
            if (this.player.factory == null)
            {
                return ;
            }
            if (objType == EObjectType.Entity)
            {
                this.player.factory.entityPool[objid].pos.x=new_pos.x;
                this.player.factory.entityPool[objid].pos.y= new_pos.y;
                this.player.factory.entityPool[objid].pos.z= new_pos.z;
            }
            if (objType == EObjectType.Prebuild)
            {
                this.player.factory.prebuildPool[objid].pos.x=new_pos.x;
                this.player.factory.prebuildPool[objid].pos.y= new_pos.y;
                this.player.factory.prebuildPool[objid].pos.z= new_pos.z;
            }
            if (objType == EObjectType.Craft)
            {
                this.player.factory.craftPool[objid].pos.x= new_pos.x;
                this.player.factory.craftPool[objid].pos.y= new_pos.y;
                this.player.factory.craftPool[objid].pos.z= new_pos.z;
            }
            if (objType == EObjectType.Vegetable)
            {
                this.player.factory.vegePool[objid].pos.x= new_pos.x;
                this.player.factory.vegePool[objid].pos.y= new_pos.y;
                this.player.factory.vegePool[objid].pos.z= new_pos.z;
            }
            if (objType == EObjectType.Vein)
            {
                this.player.factory.veinPool[objid].pos.x= new_pos.x;
                this.player.factory.veinPool[objid].pos.y= new_pos.y;
                this.player.factory.veinPool[objid].pos.z= new_pos.z;
                this.player.factory.RefreshVeinMiningDisplay(this.player.factory.veinPool[objid].id, 0, 0);
            }
            if (objType == EObjectType.Enemy)
            {
                this.player.factory.enemyPool[objid].pos.x= new_pos.x;
                this.player.factory.enemyPool[objid].pos.y= new_pos.y;
                this.player.factory.enemyPool[objid].pos.z= new_pos.z;
            }
        }
        public bool MoveResourceGroupToPos(int groupID, Vector3 new_pos)
        {
            if (groupID <= 0) return false;
            var pool = this.player.planetData.factory.veinPool;
            var veinPool = pool;
            var grps = this.player.planetData.factory.veinGroups;
            var pd = this.player.factory.planet;
            /*for (int i = 0; i < grps.Length; i++)
            {
                var g = grps[i];
            }*/
            int cnt = 0;
            for (int i = 0; i < pool.Length; i++)
            {
                var v = pool[i];
                if (v.groupIndex == groupID)
                {
                    SetObjectPos(EObjectType.Vein, i, new_pos);
                    int VeinId = v.id;
                    if (this.player.factory.planet.physics != null)
                    {
                        int colliderId = pool[i].colliderId;
                        pd.physics.RemoveColliderData(colliderId);
                        //添加碰撞信息，如果不添加，则必须飞出当前星球超过1000m之后再回头，或者退出游戏重新加载存档进入，才能更新矿脉显示
                        veinPool[i].colliderId = pd.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));

                        pd.factoryModel.gpuiManager.AlterModel((int)veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));
                    }
                    cnt++;
                    /*
                    if (this.player.factory.planet.physics != null)
                    {
                        Print_Message.Print(string.Format("[SetPlanetPhysicsColliderDirty]"));
                        this.player.factory.planet.physics.SetPlanetPhysicsColliderDirty();
                        //this.player.factory.planet.physics.LateUpdate();
                        //this.player.factory.planet.physics.Update();
                    }*/
                }
            }
            if (cnt == 0)
            {
                Print_Message.Print(string.Format("[ERROR]The Resource Group has not vein items"));
                return false;
            }
            Print_Message.Print(string.Format("[ALL_DONE]The Resource Group [id:{0} type:{1} amount:{2} vein_count:{3}] has been moved to position:({4},{5},{6})",
                groupID, grps[groupID].type, grps[groupID].amount, grps[groupID].count,new_pos.x,new_pos.y,new_pos.z));
            after_work_of_veins();
            return true;
        }
        /*
        private void changeveingrouppos(VeinData vd)
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            RaycastHit raycastHit1;
            if (pd == null || !Physics.Raycast(GameMain.mainPlayer.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            Vector3 raycastpos = raycastHit1.point;
            VeinData[] veinPool = pd.factory.veinPool;
            int colliderId;
            Vector3 begin = veinPool[vd.id].pos;
            int index = 0;
            foreach (VeinData vd1 in veinPool)
            {
                if (vd1.pos == null || vd1.id <= 0) continue;
                int VeinId = vd1.id;
                if (vd1.groupIndex == vd.groupIndex)
                {
                    Vector3 temp = PostionCompute(begin, raycastpos, vd1.pos, index++, vd.type == EVeinType.Oil);
                    if (Vector3.Distance(temp, vd1.pos) < 0.01) continue;
                    veinPool[VeinId].pos = temp;
                    if (float.IsNaN(veinPool[VeinId].pos.x) || float.IsNaN(veinPool[VeinId].pos.y) || float.IsNaN(veinPool[VeinId].pos.z))
                    {
                        continue;
                    }
                    colliderId = veinPool[VeinId].colliderId;
                    pd.physics.RemoveColliderData(colliderId);
                    veinPool[VeinId].colliderId = pd.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));

                    pd.factoryModel.gpuiManager.AlterModel((int)veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));

                }
            }

            pd.veinGroups[veinPool[vd.id].groupIndex].pos = veinPool[vd.id].pos / (pd.realRadius + 2.5f);
        }*/
        public bool SetAmoutOfResourceGroup(int groupID)
        {
            if (groupID <= 0) return false;
            var pool = this.player.planetData.factory.veinPool;
            var grps = this.player.planetData.factory.veinGroups;
            /*for (int i = 0; i < grps.Length; i++)
            {
                var g = grps[i];
            }*/
            int cnt = 0;
            for (int i = 0; i < pool.Length; i++)
            {
                var v = pool[i];
                if (v.groupIndex == groupID)
                {
                    pool[i].amount = 500000;
                    cnt++;
                }
            }
            if (cnt == 0)
            {
                Print_Message.Print(string.Format("[ERROR]The Resource Group has not vein items"));
                return false;
            }
            Print_Message.Print(string.Format("[ALL_DONE]The Resource Group [id:{0} type:{1} amount:{2} vein_count:{3}] amount has been set",
                groupID,grps[groupID].type, grps[groupID].amount, grps[groupID].count));
            after_work_of_veins();
            return true;
        }
        public bool AddVeinToResourceGroup(int groupID,int vein_item_count_to_add)
        {//在当前选定资源组中，添加8个同类型的矿脉
            if (groupID <= 0) return false;
            var pool = this.player.planetData.factory.veinPool;
            var grps = this.player.planetData.factory.veinGroups;
            var pd = this.player.factory.planet;
            /*for (int i = 0; i < grps.Length; i++)
            {
                var g = grps[i];
            }*/
            int cnt = 0;
            for (int i = 0; i < pool.Length; i++)
            {
                var v = pool[i];
                if (v.groupIndex == groupID)
                {
                    cnt++;

                    VeinProto veinProto = LDB.veins.Select((int)v.type);
                    ColliderData[] colliders = veinProto.prefabDesc.colliders;
                    for (int j = 0; j < vein_item_count_to_add; j++)
                    {
                        var new_vein = new VeinData();
                        new_vein = v;
                        new_vein.amount = 20000;
                        new_vein.id=this.player.planetData.factory.AddVeinData(new_vein);

                        int num2 = 0;
                        while (colliders != null && num2 < colliders.Length)
                        {//添加碰撞信息，如果不添加，则必须飞出当前星球超过1000m之后再回头，或者退出游戏重新加载存档进入，才能更新矿脉显示
                            new_vein.colliderId = pd.physics.AddColliderData(colliders[num2].BindToObject(new_vein.id, 0, EObjectType.Vein, new_vein.pos, Quaternion.FromToRotation(Vector3.up, new_vein.pos.normalized)));
                            num2++;
                        }
                    }
                    break;
                }
            }
            if (cnt == 0)
            {
                Print_Message.Print(string.Format("[ERROR]The Resource Group has not vein items"));
                return false;
            }
            Print_Message.Print(string.Format("[ALL_DONE]The Resource Group [id:{0} type:{1} amount:{2} vein_count:{3}] vein count has been added {4}",
                groupID, grps[groupID].type, grps[groupID].amount, grps[groupID].count,vein_item_count_to_add));
            after_work_of_veins();
            return true;
        }
        public bool SetTypeOfResourceGroup(int groupID)
        {
            //通过DSP_Storage_Vault::ce_mod_data_exchange这个全局静态数组，来和CE进行数据交换。目前暂定分配[2]这一下标对应的项，给本功能使用。接收来自用户输入的矿物类型ID值
            if (groupID <= 0) return false;
            VeinProto[] veinProtos = PlanetModelingManager.veinProtos;
            int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
            int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
            int[] veinProducts = PlanetModelingManager.veinProducts;
            PlanetRawData data = this.player.planetData.data;
            var pool = this.player.planetData.factory.veinPool;
            var grps = this.player.planetData.factory.veinGroups;
            int cnt = 0;
            /*for (int i = 0; i < grps.Length; i++)
            {
                var g = grps[i];
            }*/
            int newType = (int)DSP_Storage_Vault.ce_mod_data_exchange[2];
            Print_Message.Print(string.Format("[SetTypeOfResourceGroup]The Resource Group [id:{0} type:{1} amount:{2} vein_count:{3}] vein new TypeID is: {4}",
                groupID, grps[groupID].type, grps[groupID].amount, grps[groupID].count, newType));
            for (int i = 0; i < pool.Length; i++)
            {
                var v = pool[i];
                if (v.groupIndex == groupID)
                {
                    pool[i].type = (EVeinType)newType;//修改矿脉类型
                    pool[i].productId = veinProducts[newType];//修改产出物的类型
                    pool[i].modelIndex = (short)veinModelIndexs[newType];// 修改外观模型类型 dotNet35Random2.Next(veinModelIndexs[num20], veinModelIndexs[num20] + veinModelCounts[num20]);
                    //pool[i].amount = 10001;
                    grps[v.groupIndex].type = (EVeinType)newType;

                    cnt++;
                }
            }
            Print_Message.Print(string.Format("[ALL_DONE]The Resource Group [id:{0} type:{1} amount:{2} vein_count:{3}] type has been changed",
                groupID, grps[groupID].type, grps[groupID].amount, grps[groupID].count));
            after_work_of_veins();
            return true;
        }
        public bool AddResourceGroup()
        {
            int vein_item_count_to_add = 30; //一次性创建一个含有多少个子矿脉的矿脉资源组
            //通过DSP_Storage_Vault::ce_mod_data_exchange这个全局静态数组，来和CE进行数据交换。目前暂定分配[2]这一下标对应的项，给本功能使用。接收来自用户输入的矿物类型ID值
            if (DSP_Storage_Vault.ce_mod_data_exchange[2] == null)
            {
                //因为第一次使用时，该项为null，首先要装箱一个int对象进来，然后CE中才可以修改它的值
                int a = 10000;
                DSP_Storage_Vault.ce_mod_data_exchange[2] = (object)a;
                return false;
            }
            int newType = (int)DSP_Storage_Vault.ce_mod_data_exchange[2];//从CE中取得需要创建的矿脉的类型

            int newGrpIdx = -1;
            if (newType > (int)EVeinType.None && newType < (int)EVeinType.Max)
            {
                VeinGroup newGroup = new VeinGroup();//VeinGroup 是一个struct，将会是“值访问”的方式
                newGroup.SetNull();
                newGroup.count = vein_item_count_to_add;
                newGroup.amount = 1;//这里的总量是各个子矿脉的矿物数量之和
                newGroup.type = (EVeinType)newType;
                newGroup.pos = this.player.position;
                newGrpIdx = this.player.factory.AddVeinGroup(newGroup);
                this.player.factory.ArrangeVeinGroups();
                int groupID = newGrpIdx;
                Print_Message.Print(string.Format("[AddResourceGroup] Add The New Resource Group [newGroupID:{0}] vein new TypeID is: {1}",
                    groupID, newType));
            }
            else
            {
                Print_Message.Print(string.Format("[ERROR]The Resource Type is wrong. must be a int value between {0} and {1}.", (int)EVeinType.None, (int)EVeinType.Max));
                return false;
            }

            VeinData veinData = default(VeinData);
            EVeinType eveinType2 = (EVeinType)newType;
            VeinProto[] veinProtos = PlanetModelingManager.veinProtos;
            int[] veinModelIndexs = PlanetModelingManager.veinModelIndexs;
            int[] veinModelCounts = PlanetModelingManager.veinModelCounts;
            int[] veinProducts = PlanetModelingManager.veinProducts;
            //PlanetRawData data = this.player.planetData.data;
            var pd = this.player.factory.planet;
            for (int n = 0; n < vein_item_count_to_add; n++)
            {
                //Vector3 b = (this.tmp_vecs[n].x * a + this.tmp_vecs[n].y * a2) * num;
                veinData.type = eveinType2;
                veinData.groupIndex = (short)newGrpIdx;
                veinData.modelIndex = (short)veinModelIndexs[newType];// dotNet35Random2.Next(veinModelIndexs[num20], veinModelIndexs[num20] + veinModelCounts[num20]);
                veinData.amount = 210001;
                veinData.productId = veinProducts[newType];
                veinData.pos = this.player.position;
                veinData.minerCount = 0;
                //float num29 = data.QueryHeight(veinData.pos);
                //data.EraseVegetableAtPoint(veinData.pos);
                //veinData.pos = veinData.pos.normalized * num29;
                VeinData[] veinPool = this.player.planetData.factory.veinPool;
                //data.AddVeinData(veinData); // maybe
                int num = this.player.planetData.factory.AddVeinData(veinData);//get new Idx in veinPool
                veinData.id = num;

                //Print_Message.Print(string.Format("now vein pool length:{0}",veinPool.Length));
                veinPool[num].modelId = this.player.planetData.factoryModel.gpuiManager.AddModel((int)veinPool[num].modelIndex, num, veinPool[num].pos, new Quaternion(), true);
                //Print_Message.Print(string.Format("new vein idx:{0}", num));
                //VeinProto veinProto = LDB.veins.Select(this.handPlantPreview.protoId);
                /*VeinProto veinProto = LDB.veins.Select(this.handVeinProtoId);
                ColliderData[] colliders = veinProto.prefabDesc.colliders;
                int num2 = 0;
                while (colliders != null && num2 < colliders.Length)
                {
                    veinPool[num].colliderId = this.planet.physics.AddColliderData(colliders[num2].BindToObject(num, veinPool[num].colliderId, EObjectType.Vein, veinPool[num].pos, Quaternion.FromToRotation(Vector3.up, veinPool[num].pos.normalized)));
                    num2++;
                }*/
                VeinProto veinProto = LDB.veins.Select(newType);
                ColliderData[] colliders = veinProto.prefabDesc.colliders;
                int num2 = 0;
                while (colliders != null && num2 < colliders.Length)
                {//添加碰撞信息，如果不添加，则必须飞出当前星球超过1000m之后再回头，或者退出游戏重新加载存档进入，才能更新矿脉显示
                    veinData.colliderId = pd.physics.AddColliderData(colliders[num2].BindToObject(veinData.id, 0, EObjectType.Vein, veinData.pos, Quaternion.FromToRotation(Vector3.up, veinData.pos.normalized)));
                    num2++;
                }
                this.player.planetData.factory.RefreshVeinMiningDisplay(num, 0, 0);
            }

            this.player.planetData.factory.RecalculateVeinGroup(newGrpIdx);

            this.player.factory.ArrangeVeinGroups();
            current_group_idx = newGrpIdx;
            DSP_Storage_Vault.ce_mod_data_exchange[4] = (object)current_group_idx;
            /*
            if (cnt == 0)
            {
                Print_Message.Print(string.Format("[ERROR]The Resource Group has not vein items"));
                return false;
            }*/
            //Print_Message.Print(string.Format("[ALL_DONE]The Resource Group [id:{0} type:{1} amount:{2} vein_count:{3}] type has been changed",
            //    groupID, grps[groupID].type, grps[groupID].amount, grps[groupID].count));
            after_work_of_veins();
            return true;
        }
        private void after_work_of_veins()
        {
            this.player.planetData.factory.RecalculateAllVeinGroups();
            //this.player.controller.actionPlant.PlantVeinFinally();
        }
        private static int is_hot_key_set = 0;
        private static int current_group_idx = 0;
        private static int last_planet_id = 0;
        //public static object[] exchange_addresses = new object[1024*10];
        //这里留下这样的特征，是为了让CE能够通过内存寻找的方式，找到这个数组，然后进行内存数据的交换
        // PlayerAction_Inspect  
        // Token: 0x06000D7A RID: 3450 RVA: 0x000E30A0 File Offset: 0x000E12A0
        //VFAudio.Create("ui-click-0", null, Vector3.zero, true, 2, -1, -1L);
#if true
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_SetInspectee(EObjectType objType, int objId)
        {
            old_SetInspectee(objType, objId);
#if true
            if (objId == 0)
            {
                this.InspectNothing();
                return;
            }
            if (this.player.factory == null)
            {
                this.InspectNothing();
                return;
            }
            if (objType == EObjectType.Entity)
            {
                objId = this.player.factory.entityPool[objId].id;
                /*var item_proto = this.GetItemProto(objId, this.player.factory);
                if (item_proto != null)
                {
                    exchange_addresses[2] = item_proto.prefabDesc;
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                }
                else*/
                {
                    //Print_Message.Print(string.Format("[entityPool]111...."));
                    var mdidx = this.player.factory.entityPool[objId].modelIndex;
                    //Print_Message.Print(string.Format("[entityPool]222...."));
                    ModelProto modelProto = LDB.models.Select(mdidx);
                    //Print_Message.Print(string.Format("[entityPool]333...."));
                    if (modelProto != null)
                    {
                        //VFAudio.Create("ui-error", null, Vector3.zero, true);
                        DSP_Storage_Vault.ce_mod_data_exchange[1] = modelProto.prefabDesc;
                    }
                }
                /*
                var prefab = item_proto.prefabDesc;
                GCHandle handle = GCHandle.Alloc(item_proto.prefabDesc, GCHandleType.Pinned);

                try
                {
                    IntPtr address = handle.AddrOfPinnedObject();
                    //Console.WriteLine("Address: " + address);
                    exchange_addresses[2] = address.ToInt64();
                }
                finally
                {
                    handle.Free();
                }*/
            }
            if (objType == EObjectType.Craft)
            {
                objId = this.player.factory.craftPool[objId].id;
                var mdidx = this.player.factory.craftPool[objId].modelIndex;
                ModelProto modelProto = LDB.models.Select(mdidx);
                //Print_Message.Print(string.Format("[entityPool]333...."));
                if (modelProto != null)
                {
                    //VFAudio.Create("ui-error", null, Vector3.zero, true);
                    DSP_Storage_Vault.ce_mod_data_exchange[1] = modelProto.prefabDesc;
                }
                VFAudio.Create("ui-click-2", null, Vector3.zero, true);
            }
            if (objType == EObjectType.Vegetable)
            {
                objId = this.player.factory.vegePool[objId].id;
            }
            if (objType == EObjectType.Vein)
            {
                objId = this.player.factory.veinPool[objId].id;
                var mdidx = this.player.factory.veinPool[objId].modelIndex;
                ModelProto modelProto = LDB.models.Select(mdidx);
                if (modelProto != null)
                {
                    //VFAudio.Create("ui-error", null, Vector3.zero, true);
                    DSP_Storage_Vault.ce_mod_data_exchange[1] = modelProto.prefabDesc;
                }

                /*
                Print_Message.Print(string.Format("Hot key Enable State...{0}", is_hot_key_set));
                if (is_hot_key_set == 0)
                {
                    KeyEvtMgr.key_evt.RegisterKeyEventHandler(onKeyResp);
                    Print_Message.Print(string.Format("[SUCCESS]Enable Hot key for this mod...."));
                    is_hot_key_set = 1;
                }
                */
                //regist_key_event();

                var grps = this.player.planetData.factory.veinGroups;
                current_group_idx = this.player.factory.veinPool[objId].groupIndex;
                if(current_group_idx<=0||current_group_idx>= grps.Length)
                {
                    string str2 = string.Format("当前操作选定了无效的资源组【{0}】",current_group_idx);
                    current_group_idx = 0;
                    DSP_Storage_Vault.ce_mod_data_exchange[4] = (object)current_group_idx;
                    DSP_Storage_Vault.ce_mod_data_exchange[2] = (object)current_group_idx;
                    PopupMessage(str2);
                    Print_Message.Print(str2);
                    return;
                }
                DSP_Storage_Vault.ce_mod_data_exchange[4] = (object)current_group_idx;
                DSP_Storage_Vault.ce_mod_data_exchange[2] = (object)((int)this.player.planetData.factory.veinGroups[current_group_idx].type);
                Print_Message.Print(string.Format("==>Mouse click on Current Resource Group [groupID: {0} type:{1} amount:{2} vein_count:{3}]",
                    current_group_idx, grps[current_group_idx].type, grps[current_group_idx].amount, grps[current_group_idx].count));
            }
            if (objType == EObjectType.Enemy)
            {
                objId = this.player.factory.enemyPool[objId].id;
            }
            if (objId == 0)
            {
                this.InspectNothing();
                return;
            }
            Vector3 objectPos = this.GetObjectPos(objType, objId);

            Print_Message.Print(string.Format("[CLICK on object]Player Mecha to Object distance=>{0},the Object position:({1},{2},{3})", (this.player.position - objectPos).magnitude, objectPos.x, objectPos.y, objectPos.z));
            /*
            float objectSelectDistance = this.GetObjectSelectDistance(objType, objId);
            if ((this.player.position - objectPos).sqrMagnitude <= objectSelectDistance * objectSelectDistance)
            {
            }*/
#endif
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_SetInspectee(EObjectType objType, int objId)
        {
        }
#endif
    }
}
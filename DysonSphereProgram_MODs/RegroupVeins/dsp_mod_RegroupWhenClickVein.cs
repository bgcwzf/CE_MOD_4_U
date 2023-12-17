using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;


//MOD_PATCH_TARGET==>PlayerAction_Inspect:SetInspectee
//MOD_NEW_METHOD==>DSP_CE_MOD.RegouopVeins_PlayerAction_Inspect:new_SetInspectee
//MOD_OLD_CALLER==>DSP_CE_MOD.RegouopVeins_PlayerAction_Inspect:old_SetInspectee
//MOD_DESCRIPTION==>a MONO MOD DLL
namespace DSP_CE_MOD
{
    public class RegouopVeins_PlayerAction_Inspect : PlayerAction_Inspect
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void onKeyResp(KeyCode keyCode)
        {
            Print_Message.Print(string.Format("RegouopVeins_PlayerAction_Inspect ...onKeyResp(KeyCode:{0})",keyCode));
            switch (keyCode)
            {
                case KeyCode.F1:
                    if (Input.GetKey(KeyCode.LeftControl))
                    {//按下Ctrl+F1的时候，就是卸载当前键盘事件处理的代理程序
                        KeyEvtMgr.key_evt.UnregisterKeyEventHandler(onKeyResp);
                        is_hot_key_set = 0;
                        Print_Message.Print(string.Format("KeyEvtMgr.key_evt.UnregisterKeyEventHandler() ... unloaded...."));
                        VFAudio.Create("ui-error", null, Vector3.zero, true);
                        break;
                    }
                    //正式开始执行矿脉重排的任务
                    ArrangeVeins();
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);//, 5, -1, -1L);
                    break;
                case KeyCode.F2:
                    //初始化当前的groupIdx=1
                    current_group_idx = 1;
                    Print_Message.Print(string.Format("Current Resource Group ID: {0}",current_group_idx));
                    break;
                case KeyCode.F3:
                    if (current_group_idx - 1 <= 0 )
                    {
                        VFAudio.Create("ui-error", null, Vector3.zero, true);
                        break;
                    }
                    current_group_idx--;
                    Print_Message.Print(string.Format("Current Resource Group ID: {0}", current_group_idx));
                    break;
                case KeyCode.F4:
                    if (current_group_idx+1 >= this.player.planetData.factory.veinGroups.Length)
                    {
                        VFAudio.Create("ui-error", null, Vector3.zero, true);
                        break;
                    }
                    current_group_idx++;
                    Print_Message.Print(string.Format("Current Resource Group ID: {0}", current_group_idx));
                    break;
                case KeyCode.F5:
                    //将当前选定资源组，移动到机甲所在的当前位置
                    Print_Message.Print(string.Format("Current Resource Group ID: {0}", current_group_idx));
                    if (MoveResourceGroupToPos(current_group_idx, this.player.position))
                        VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                    else
                        VFAudio.Create("ui-error", null, Vector3.zero, true);
                    break;
                case KeyCode.F6:
                    //将当前选定资源组的矿产储量翻倍
                    Print_Message.Print(string.Format("Current Resource Group ID: {0}", current_group_idx));
                    if (SetAmoutOfResourceGroup(current_group_idx))
                        VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                    else
                        VFAudio.Create("ui-error", null, Vector3.zero, true);
                    break;
                case KeyCode.F7:
                    //在当前选定资源组中，添加8个同类型的矿脉
                    Print_Message.Print(string.Format("Current Resource Group ID: {0}", current_group_idx));
                    if (AddVeinToResourceGroup(current_group_idx, 8))
                        VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                    else
                        VFAudio.Create("ui-error", null, Vector3.zero, true);
                    break;
                case KeyCode.F8:
                    //针对资源的临时特定操作
                    break;
                case KeyCode.F9:
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
            // 注意，这里有一个巨大的“天坑”！！！！
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
                    SetObjectPos(EObjectType.Vein, i, new_pos);
                    cnt++;
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
                    pool[i].amount = 200000;
                    cnt++;
                }
            }
            if (cnt == 0)
            {
                Print_Message.Print(string.Format("[ERROR]The Resource Group has not vein items"));
                return false;
            }
            Print_Message.Print(string.Format("[ALL_DONE]The Resource Group [id:{0} type:{1} amount:{2} vein_count:{3}] amount has been doubled",
                groupID,grps[groupID].type, grps[groupID].amount, grps[groupID].count));
            after_work_of_veins();
            return true;
        }
        public bool AddVeinToResourceGroup(int groupID,int vein_item_count_to_add)
        {//在当前选定资源组中，添加8个同类型的矿脉
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
                    cnt++;

                    for (int j = 0; j < vein_item_count_to_add; j++)
                    {
                        var new_vein = new VeinData();
                        new_vein = v;
                        new_vein.amount = 20000;
                        this.player.planetData.factory.AddVeinData(new_vein);
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
        private void after_work_of_veins()
        {
            this.player.planetData.factory.RecalculateAllVeinGroups();
            //this.player.controller.actionPlant.PlantVeinFinally();
        }
        private int is_hot_key_set = 0;
        private int current_group_idx = -1;
        // PlayerAction_Inspect  
        // Token: 0x06000D7A RID: 3450 RVA: 0x000E30A0 File Offset: 0x000E12A0
        //VFAudio.Create("ui-click-0", null, Vector3.zero, true, 2, -1, -1L);
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
            }
            if (objType == EObjectType.Craft)
            {
                objId = this.player.factory.craftPool[objId].id;
            }
            if (objType == EObjectType.Vegetable)
            {
                objId = this.player.factory.vegePool[objId].id;
            }
            if (objType == EObjectType.Vein)
            {
                objId = this.player.factory.veinPool[objId].id;

                Print_Message.Print(string.Format("Hot key Enable State...{0}", is_hot_key_set));
                if (is_hot_key_set == 0)
                {
                    KeyEvtMgr.key_evt.RegisterKeyEventHandler(onKeyResp);
                    Print_Message.Print(string.Format("[SUCCESS]Enable Hot key for this mod...."));
                    is_hot_key_set = 1;
                }

                current_group_idx = this.player.factory.veinPool[objId].groupIndex;
                var grps = this.player.planetData.factory.veinGroups;
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
    }
}
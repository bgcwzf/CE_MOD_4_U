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
                    //先用鼠标左键，选定某个对象（矿脉、树木、地面的石头、建筑等实体），然后按下F2键，即可将其移动到当前机甲所在的位置。
                    SetObjectPos(this.inspectType, this.inspectId, this.player.position);
                    Print_Message.Print(string.Format("Player position:({1},{2},{3})",0, this.player.position.x, this.player.position.y, this.player.position.z));
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                    //this.player.planetData.Load();
                    //VFAudio.Create("ui-error", null, Vector3.zero, true, 5, -1, -1L);
                    break;
                case KeyCode.F3:
                    //每按一次就修改当前星球上的每一个矿物的储量为现有储量的2倍
                    Print_Message.Print(string.Format("Before DoubleTheAmountOfVein()"));
                    DoubleTheAmountOfVein();
                    Print_Message.Print(string.Format("After DoubleTheAmountOfVein()"));
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);
                    //this.player.planetData.LoadFactory();
                    //VFAudio.Create("ui-click-1", null, Vector3.zero, true, 5, -1, -1L);
                    break;
                case KeyCode.F4:
                    //寻找石油矿在veinPool中的序号
                    init_oil_idx_in_vein_pool();
                    Print_Message.Print(string.Format("Current oil vein index: {0} [F4], min={1}, max={2}", oil_idx_in_vein_pool,oil_idx_min,oil_idx_max));
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);//, 5, -1, -1L);
                    break;
                case KeyCode.F5:
                    oil_idx_in_vein_pool++;
                    Print_Message.Print(string.Format("Current oil vein index: {0} [F5], min={1}, max={2}", oil_idx_in_vein_pool, oil_idx_min, oil_idx_max));
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);//, 5, -1, -1L);
                    break;
                case KeyCode.F6:
                    oil_idx_in_vein_pool--;
                    Print_Message.Print(string.Format("Current oil vein index: {0} [F6], min={1}, max={2}", oil_idx_in_vein_pool, oil_idx_min, oil_idx_max));
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);//, 5, -1, -1L);
                    break;
                case KeyCode.F7:
                    //移动选定的石油矿
                    Print_Message.Print(string.Format("before set oil vein [{0}] position...., min={1}, max={2}", oil_idx_in_vein_pool, oil_idx_min, oil_idx_max));
                    if (this.player.planetData.factory.veinGroups[this.player.planetData.factory.veinPool[oil_idx_in_vein_pool].groupIndex].type!= EVeinType.Oil)
                    {
                        Print_Message.Print(string.Format("error set oil vein [{0}] position....the vein type is not oil", oil_idx_in_vein_pool));
                        break;
                    }
                    SetObjectPos(EObjectType.Vein,oil_idx_in_vein_pool,this.player.position);
                    Print_Message.Print(string.Format("after set oil vein [{0}] position...., min={1}, max={2}", oil_idx_in_vein_pool, oil_idx_min, oil_idx_max));
                    VFAudio.Create("ui-click-2", null, Vector3.zero, true);//, 5, -1, -1L);
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
                Print_Message.Print(string.Format("[ALL_DONE]This Vein=>id:{0},group index:{1},amount:{2},pos:({3},{4},{5})", v.id, v.groupIndex, v.amount, v.pos.x, v.pos.y, v.pos.z));
            }
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
        public void DoubleTheAmountOfVein()
        {
            var pool = this.player.planetData.factory.veinPool;
            var grps = this.player.planetData.factory.veinGroups;

            for (int i = 0; i < pool.Length; i++)
            {
                var v = pool[i];
                //Print_Message.Print(string.Format("This Vein=>id:{0},group index:{1},amount:{2},pos:({3},{4},{5})", v.id,v.groupIndex,v.amount,v.pos.x,v.pos.y,v.pos.z));
                if (v.id <= 0 || v.groupIndex <= 0 || v.amount <= 0 || (grps[v.groupIndex].type == EVeinType.None) || grps[v.groupIndex].count <= 0) continue;
                if (pool[i].amount < 5000) pool[i].amount = 5000;
                pool[i].amount *= 2;
                if(grps[v.groupIndex].count<12)
                pool[i].amount *= (int)(12.0f / grps[v.groupIndex].count);
            }
        }
        private void init_oil_idx_in_vein_pool()
        {
            var pool = this.player.planetData.factory.veinPool;
            var grps = this.player.planetData.factory.veinGroups;

            for (int i = 0; i < pool.Length; i++)
            {
                var v = pool[i];

                if (v.id <= 0 || v.groupIndex <= 0 || v.amount <= 0 || (grps[v.groupIndex].type == EVeinType.None) || grps[v.groupIndex].count <= 0) continue;
                if(grps[v.groupIndex].type == EVeinType.Oil)
                {
                    oil_idx_min=oil_idx_in_vein_pool = i;
                    break;
                }
            }
            if (oil_idx_in_vein_pool <= 0)
            {
                Print_Message.Print(string.Format("Error, not found valid oil_idx_in_vein_pool...."));
                return;
            }
            for (int i = pool.Length-1; i >=0; i--)
            {
                var v = pool[i];

                if (v.id <= 0 || v.groupIndex <= 0 || v.amount <= 0 || (grps[v.groupIndex].type == EVeinType.None) || grps[v.groupIndex].count <= 0) continue;
                if (grps[v.groupIndex].type == EVeinType.Oil)
                {
                    oil_idx_max = i;
                    break;
                }
            }
            Print_Message.Print(string.Format("Success find range of oil_idx_in_vein_pool: [{0},{1}]", oil_idx_min, oil_idx_max));
        }
        private int is_hot_key_set = 0;
        private int oil_idx_in_vein_pool = -1;
        private int oil_idx_min = -1, oil_idx_max = -1;
        // PlayerAction_Inspect  
        // Token: 0x06000D7A RID: 3450 RVA: 0x000E30A0 File Offset: 0x000E12A0
        //VFAudio.Create("ui-click-0", null, Vector3.zero, true, 2, -1, -1L);
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_SetInspectee(EObjectType objType, int objId)
        {
            old_SetInspectee(objType, objId);
            Print_Message.Print(string.Format("Hot key Enable State...{0}",is_hot_key_set));
            if (is_hot_key_set == 0)
            {
                KeyEvtMgr.key_evt.RegisterKeyEventHandler(onKeyResp);
                Print_Message.Print(string.Format("Enable Hot key for this mod.......success"));
                is_hot_key_set = 1;
            }

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
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>UIStationStorage:OnItemIconMouseDown|StorageComponent:Import|PlayerAction_Inspect:GetObjectSelectDistance
//MOD_NEW_METHOD==>DSP_CE_MOD.My_UIStationStorage:new_OnItemIconMouseDown|DSP_CE_MOD.StorageComponent_StackSizeChange:new_Import|DSP_CE_MOD.PlayerAction_Inspect_Select_From_Far:new_GetObjectSelectDistance
//MOD_OLD_CALLER==>DSP_CE_MOD.My_UIStationStorage:old_OnItemIconMouseDown|DSP_CE_MOD.StorageComponent_StackSizeChange:old_Import|DSP_CE_MOD.PlayerAction_Inspect_Select_From_Far:old_GetObjectSelectDistance
//MOD_DESCRIPTION==>用于演示的第一个CE的MONO MOD DLL
namespace DSP_CE_MOD
{
    public class My_UIStationStorage : UIStationStorage
    {
        // UIStationStorage
        // Token: 0x06002B5D RID: 11101 RVA: 0x001F8F80 File Offset: 0x001F7180
        public void new_OnItemIconMouseDown(BaseEventData evt)
        {
            PointerEventData pointerEventData = evt as PointerEventData;
            if (pointerEventData == null)
            {
                return;
            }
            var pv_insplit = PrivateHelper<UIStationStorage>.GetPrivateField("insplit");
            //var pv_split_inc = PrivateHelper<UIStationStorage>.GetPrivateMethod("split_inc");
            Player mainPlayer = GameMain.mainPlayer;
            if (mainPlayer.inhandItemId == 0)
            {
                if (pointerEventData.button == PointerEventData.InputButton.Right)
                {
                    int count = this.station.storage[this.index].count;
                    if (count > 0)
                    {
                        UIRoot.instance.uiGame.OpenGridSplit(this.station.storage[this.index].itemId, count, Input.mousePosition);
                        //this.insplit = true;
                        pv_insplit.SetValue(this, true);
                        return;
                    }
                }
            }
            else if (mainPlayer.inhandItemId == this.station.storage[this.index].itemId && pointerEventData.button == PointerEventData.InputButton.Left)
            {
                ItemProto itemProto = LDB.items.Select((int)this.stationWindow.factory.entityPool[this.station.entityId].protoId);
                int additionStorage = this.GetAdditionStorage();
                // org code:
                //int num = ((itemProto != null) ? (itemProto.prefabDesc.stationMaxItemCount + additionStorage) : storage_max) - this.station.storage[this.index].count;

                var storage_max = 100000000;//this.station.storage[this.index].max;
                int num = storage_max - this.station.storage[this.index].count;
                int num2 = (num < mainPlayer.inhandItemCount) ? num : mainPlayer.inhandItemCount;
                if (num2 < 0)
                {
                    if (mainPlayer.inhandItemCount <= 0)
                    {
                        mainPlayer.SetHandItems(0, 0, 0);
                    }
                    return;
                }
                int inhandItemCount = mainPlayer.inhandItemCount;
                int inhandItemInc = mainPlayer.inhandItemInc;
                int num3 = num2;
                int num4 = 0;

                object [] method_params=new object[3];
                method_params[0]= inhandItemCount;
                method_params[1]= inhandItemInc;
                method_params[2] = num3;
                //num4=this.split_inc(ref inhandItemCount, ref inhandItemInc, num3);
                num4=(int)PrivateHelper<UIStationStorage>.InvokeMethodRefParams("split_inc",this, method_params);
                inhandItemCount = (int)method_params[0];
                inhandItemInc = (int)method_params[1];
                /////////////////////////////////////////////
                StationStore[] storage = this.station.storage;
                int num5 = this.index;
                storage[num5].count = storage[num5].count + num3;
                StationStore[] storage2 = this.station.storage;
                int num6 = this.index;
                storage2[num6].inc = storage2[num6].inc + num4;
                mainPlayer.AddHandItemCount_Unsafe(-num3);
                mainPlayer.SetHandItemInc_Unsafe(mainPlayer.inhandItemInc - num4);
                if (mainPlayer.inhandItemCount <= 0)
                {
                    mainPlayer.SetHandItems(0, 0, 0);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_OnItemIconMouseDown(BaseEventData evt)
        {
            // 这里是一个跳板函数。在MOD被动态载入到目标进程之后，它指向Patch之前的原始函数。
            // 当需要在新函数中，对原来函数进行调用时，执行“old_OnItemIconMouseDown(evt)”即可

            // 本函数内部必须留空。
            // 函数名称可以自由定义，但是函数返回值类型与参数列表必须与原函数完全一致
        }
    }
	//所有的存储空间（背包/储物箱等），在加载存档后，对每一个格子，会使用ItemProto.StackSize来覆盖之前的设定。要在这里强制恢复一下。
    public class StorageComponent_StackSizeChange : StorageComponent
    {
        public StorageComponent_StackSizeChange(int a) : base(a) { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_Import(System.IO.BinaryReader r)
        {
            old_Import(r);
            if (this.grids == null || this.grids.Length <= 0) return;
            for (int i = 0; i < this.grids.Length; i++)
            {
                this.grids[i].stackSize = 0x7FFFFFFF - 10;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_Import(System.IO.BinaryReader r)
        {
        }
    }
    public class PlayerAction_Inspect_Select_From_Far : PlayerAction_Inspect
    {//解决了在较远地方无法对建筑物进行点选和操作的问题
        [MethodImpl(MethodImplOptions.NoInlining)]
        public float new_GetObjectSelectDistance(EObjectType objType, int objid) { return 1200.0f; }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public float old_GetObjectSelectDistance(EObjectType objType, int objid) { return 0.0f; }
    }
}
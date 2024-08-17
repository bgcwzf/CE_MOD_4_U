using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>PlayerOrder:_trimEnd|PlayerOrder:Enqueue
//MOD_NEW_METHOD==>DSP_CE_MOD.Patch_PlayerOrder:new_trimEnd|DSP_CE_MOD.Patch_PlayerOrder:new_Enqueue
//MOD_OLD_CALLER==>DSP_CE_MOD.Patch_PlayerOrder:old_trimEnd|DSP_CE_MOD.Patch_PlayerOrder:old_Enqueue
//MOD_DESCRIPTION==>【暂时禁用】尝试修改用户按住Shift键后的最大命令队列长度（默认是16个命令）
namespace DSP_CE_MOD
{
    public class Patch_PlayerOrder : PlayerOrder
    {
        public Patch_PlayerOrder(Player _player):base(_player) { }
        // UIStationStorage
        // Token: 0x06000EFA RID: 3834 RVA: 0x00108FE8 File Offset: 0x001071E8
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_PlayerOrder(Player _player)
        {
            old_PlayerOrder(_player);
            var f = PrivateHelper<PlayerOrder>.GetPrivateField("orderQueue");

            f.SetValue(this,new OrderNode[9999999]);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_PlayerOrder(Player _player)
        {
            // 这里是一个跳板函数。在MOD被动态载入到目标进程之后，它指向Patch之前的原始函数。
            // 当需要在新函数中，对原来函数进行调用时，执行这个函数即可

            // 本函数内部必须留空。
            // 函数名称可以自由定义，但是函数返回值类型与参数列表必须与原函数完全一致
            //return true;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_trimEnd()
        {
            if (this == null) return;
            
            var f = PrivateHelper<PlayerOrder>.GetPrivateField("orderQueue");
            if (f == null) return;
            var q = (OrderNode[])f.GetValue(this);
            if (q == null) return;
            Array.Clear(q, this.orderCount, 9999999 - this.orderCount);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_trimEnd() { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_Enqueue(OrderNode order)
        {
            if (order == null)
            {
                return;
            }
            if (this.orderCount == 9999999)
            {
                UIRealtimeTip.Popup("指令列表已满".Translate(), true, 0);
                return;
            }
            OrderNode[] orderQueue = this.orderQueue;
            if(orderQueue.Length!= 9999999)
            {
                var f1 = PrivateHelper<PlayerOrder>.GetPrivateField("orderQueue");
                f1.SetValue(this, new OrderNode[9999999]);
            }
            int orderCount = this.orderCount;
            var f = PrivateHelper<PlayerOrder>.GetPrivateField("orderCount");
            f.SetValue(this,orderCount+1);// this.orderCount = orderCount + 1;
            f = PrivateHelper<PlayerOrder>.GetPrivateField("orderQueue");
            var q = (OrderNode[])f.GetValue(this);
            q[orderCount] = order;//orderQueue[orderCount] = order;
            if (this.currentOrder == null)
            {
                var c = PrivateHelper<PlayerOrder>.GetPrivateField("currentOrder");
                var ret=PrivateHelper<PlayerOrder>.InvokeMethodNonRefParams("Dequeue", this);
                c.SetValue(this, ret);//this.currentOrder = this.Dequeue();
            }
            var p = PrivateHelper<PlayerOrder>.GetPrivateField("player");
            var pla = (Player)p.GetValue(this);
            pla.gizmo.AddOrderGizmo(order);//this.player.gizmo.AddOrderGizmo(order);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_Enqueue(OrderNode order) { }

    }
}
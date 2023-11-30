using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>StationComponent:UpdateNeeds
//MOD_NEW_METHOD==>DSP_CE_MOD.My_StationComponent:new_UpdateNeeds
//MOD_OLD_CALLER==>DSP_CE_MOD.My_StationComponent:old_UpdateNeeds
//MOD_DESCRIPTION==>用于演示的第一个CE的MONO MOD DLL
namespace DSP_CE_MOD
{
    public class My_StationComponent : StationComponent
    {// StationComponent
     // Token: 0x060005C8 RID: 1480 RVA: 0x00041AF8 File Offset: 0x0003FCF8
        public void new_UpdateNeeds()
        {
            var storage_max = 100000000;
            StationStore[] obj = this.storage;
            lock (obj)
            {
                int num = this.storage.Length;
                /* // org code
                this.needs[0] = ((0 < num && this.storage[0].count < this.storage[0].max) ? this.storage[0].itemId : 0);
                this.needs[1] = ((1 < num && this.storage[1].count < this.storage[1].max) ? this.storage[1].itemId : 0);
                this.needs[2] = ((2 < num && this.storage[2].count < this.storage[2].max) ? this.storage[2].itemId : 0);
                this.needs[3] = ((3 < num && this.storage[3].count < this.storage[3].max) ? this.storage[3].itemId : 0);
                this.needs[4] = ((4 < num && this.storage[4].count < this.storage[4].max) ? this.storage[4].itemId : 0);*/
                this.needs[0] = ((0 < num && this.storage[0].count < storage_max) ? this.storage[0].itemId : 0);
                this.needs[1] = ((1 < num && this.storage[1].count < storage_max) ? this.storage[1].itemId : 0);
                this.needs[2] = ((2 < num && this.storage[2].count < storage_max) ? this.storage[2].itemId : 0);
                this.needs[3] = ((3 < num && this.storage[3].count < storage_max) ? this.storage[3].itemId : 0);
                this.needs[4] = ((4 < num && this.storage[4].count < storage_max) ? this.storage[4].itemId : 0);
                this.needs[5] = ((this.isStellar && this.warperCount < this.warperMaxCount) ? 1210 : 0);
            }
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_UpdateNeeds()
        {
            // 这里是一个跳板函数。在MOD被动态载入到目标进程之后，它指向Patch之前的原始函数。
            // 当需要在新函数中，对原来函数进行调用时，执行“old_UpdateNeeds()”即可

            // 本函数内部必须留空。
            // 函数名称可以自由定义，但是函数返回值类型与参数列表必须与原函数完全一致
        }
    }
}
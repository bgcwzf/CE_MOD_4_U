using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>BuildTool_Click:CheckBuildConditions
//MOD_NEW_METHOD==>DSP_CE_MOD.DisableCollide_BuildTool_Click:new_CheckBuildConditions
//MOD_OLD_CALLER==>DSP_CE_MOD.DisableCollide_BuildTool_Click:old_CheckBuildConditions
//MOD_DESCRIPTION==>用于演示的第一个CE的MONO MOD DLL
namespace DSP_CE_MOD
{
    public class DisableCollide_BuildTool_Click : BuildTool_Click
    {
        // UIStationStorage
        // Token: 0x06000EFA RID: 3834 RVA: 0x00108FE8 File Offset: 0x001071E8
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool new_CheckBuildConditions()
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool old_CheckBuildConditions()
        {
            // 这里是一个跳板函数。在MOD被动态载入到目标进程之后，它指向Patch之前的原始函数。
            // 当需要在新函数中，对原来函数进行调用时，执行这个函数即可

            // 本函数内部必须留空。
            // 函数名称可以自由定义，但是函数返回值类型与参数列表必须与原函数完全一致
            return true;
        }
    }
}
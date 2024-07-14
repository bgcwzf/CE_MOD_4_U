using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>BuildTool_BlueprintPaste:CheckBuildConditions,BuildTool_BlueprintPaste:CheckBuildConditionsPrestage
//MOD_NEW_METHOD==>DSP_CE_MOD.DisableCollide_BuildTool_BlueprintPaste:new_CheckBuildConditions,DSP_CE_MOD.DisableCollide_BuildTool_BlueprintPaste:new_CheckBuildConditionsPrestage
//MOD_OLD_CALLER==>DSP_CE_MOD.DisableCollide_BuildTool_BlueprintPaste:old_CheckBuildConditions,DSP_CE_MOD.DisableCollide_BuildTool_BlueprintPaste:old_CheckBuildConditionsPrestage
//MOD_DESCRIPTION==>蓝图粘贴时的建筑检查
namespace DSP_CE_MOD
{
    public class DisableCollide_BuildTool_BlueprintPaste : BuildTool_BlueprintPaste
    {
        // 负责蓝图复制的，有专门的另外一个类：BuildTool_BlueprintCopy
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
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool new_CheckBuildConditionsPrestage()
        {
            return true;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool old_CheckBuildConditionsPrestage()
        {
            return true;
        }
    }
}
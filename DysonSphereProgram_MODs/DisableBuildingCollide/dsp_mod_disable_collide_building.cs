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
            if (base.buildPreviews.Count == 0)
            {
                return false;
            }
            for (int i = 0; i < base.buildPreviews.Count; i++)
            {
                BuildPreview buildPreview = base.buildPreviews[i];

                Vector3 vector = buildPreview.lpos;
                //Quaternion quaternion = buildPreview.lrot;
                Vector3 lpos = buildPreview.lpos2;
                //Quaternion lrot = buildPreview.lrot2;
                Pose lPose = new Pose(buildPreview.lpos, buildPreview.lrot);
                //Pose pose = new Pose(buildPreview.lpos2, buildPreview.lrot2);
                Vector3 forward = lPose.forward;
                //Vector3 forward2 = pose.forward;
                Vector3 up = lPose.up;
                //Vector3 a = Vector3.Lerp(vector, lpos, 0.5f);
                Vector3 forward3 = lpos - vector;
                if (forward3.sqrMagnitude < 0.0001f)
                {
                 //   forward3 = Maths.SphericalRotation(vector, 0f).Forward();
                }
                //Quaternion quaternion2 = Quaternion.LookRotation(forward3, a.normalized);
                //bool flag3 = this.planet != null && this.planet.type == EPlanetType.Gas;
                //bool flag4 = buildPreview.desc.minerType == EMinerType.None && !buildPreview.desc.isBelt && !buildPreview.desc.isSplitter && (!buildPreview.desc.isPowerNode || buildPreview.desc.isPowerGen || buildPreview.desc.isAccumulator || buildPreview.desc.isPowerExchanger) && !buildPreview.desc.isStation && !buildPreview.desc.isSilo && !buildPreview.desc.multiLevel && !buildPreview.desc.isMonitor && !buildPreview.desc.geothermal && !buildPreview.desc.isBattleBase;
                if (buildPreview.desc.veinMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    PrebuildData prebuildData = default(PrebuildData);
                    int paramCount = 0;
                    if (buildPreview.desc.isVeinCollector)
                    {
                        Vector3 center = vector.normalized * base.controller.cmd.test.magnitude + forward * -10f;
                        int veinsInAreaNonAlloc = base.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center, 18f, BuildTool._tmp_ids);
                        prebuildData.InitParametersArray(veinsInAreaNonAlloc);
                        VeinData[] veinPool = this.factory.veinPool;
                        EVeinType eveinType = EVeinType.None;
                        for (int j = 0; j < veinsInAreaNonAlloc; j++)
                        {
                            if (BuildTool._tmp_ids[j] != 0 && veinPool[BuildTool._tmp_ids[j]].id == BuildTool._tmp_ids[j])
                            {
                                if (veinPool[BuildTool._tmp_ids[j]].type != EVeinType.Oil && MinerComponent.IsTargetVeinInRange(veinPool[BuildTool._tmp_ids[j]].pos, lPose, buildPreview.desc))
                                {
                                    if (eveinType != veinPool[BuildTool._tmp_ids[j]].type)
                                    {
                                        if (eveinType == EVeinType.None)
                                        {
                                            eveinType = veinPool[BuildTool._tmp_ids[j]].type;
                                        }
                                        else
                                        {
                                            //buildPreview.condition = EBuildCondition.NeedSingleResource;
                                        }
                                    }
                                    prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[j];
                                }
                            }
                            else
                            {
                                Assert.CannotBeReached();
                            }
                        }
                    }
                    else
                    {
                        Vector3 center2 = vector.normalized * base.controller.cmd.test.magnitude + forward * -1.2f;
                        int veinsInAreaNonAlloc2 = base.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(center2, 12f, BuildTool._tmp_ids);
                        prebuildData.InitParametersArray(veinsInAreaNonAlloc2);
                        VeinData[] veinPool2 = this.factory.veinPool;
                        EVeinType eveinType2 = EVeinType.None;
                        for (int k = 0; k < veinsInAreaNonAlloc2; k++)
                        {
                            if (BuildTool._tmp_ids[k] != 0 && veinPool2[BuildTool._tmp_ids[k]].id == BuildTool._tmp_ids[k])
                            {
                                if (veinPool2[BuildTool._tmp_ids[k]].type != EVeinType.Oil && MinerComponent.IsTargetVeinInRange(veinPool2[BuildTool._tmp_ids[k]].pos, lPose, buildPreview.desc))
                                {
                                    if (eveinType2 != veinPool2[BuildTool._tmp_ids[k]].type)
                                    {
                                        if (eveinType2 == EVeinType.None)
                                        {
                                            eveinType2 = veinPool2[BuildTool._tmp_ids[k]].type;
                                        }
                                        else
                                        {
                                            //buildPreview.condition = EBuildCondition.NeedResource;
                                        }
                                    }
                                    prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[k];
                                }
                            }
                            else
                            {
                                Assert.CannotBeReached();
                            }
                        }
                    }
                    prebuildData.paramCount = paramCount;
                    prebuildData.ArrangeParametersArray();
                    if (buildPreview.desc.isVeinCollector)
                    {
                        if (buildPreview.paramCount == 0)
                        {
                            buildPreview.parameters = new int[2048];
                            buildPreview.paramCount = 2048;
                        }
                        if (prebuildData.paramCount > 0)
                        {
                            Array.Resize<int>(ref buildPreview.parameters, buildPreview.paramCount + prebuildData.paramCount);
                            Array.Copy(prebuildData.parameters, 0, buildPreview.parameters, buildPreview.paramCount, prebuildData.paramCount);
                            buildPreview.paramCount += prebuildData.paramCount;
                        }
                    }
                    else
                    {
                        buildPreview.parameters = prebuildData.parameters;
                        buildPreview.paramCount = prebuildData.paramCount;
                    }
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    if (prebuildData.paramCount == 0)
                    {
                        buildPreview.condition = EBuildCondition.NeedResource;
                        continue;//goto IL_2D8D;
                    }
                }
                else if (buildPreview.desc.oilMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    Vector3 vector2 = vector;
                    Vector3 vector3 = -up;
                    int veinsInAreaNonAlloc3 = base.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector2, 10f, BuildTool._tmp_ids);
                    PrebuildData prebuildData2 = default(PrebuildData);
                    prebuildData2.isDestroyed = false;
                    prebuildData2.InitParametersArray(veinsInAreaNonAlloc3);
                    VeinData[] veinPool3 = this.factory.veinPool;
                    int num2 = 0;
                    float num3 = 10f;//org: 100f;
                    Vector3 pos = vector2;
                    for (int l = 0; l < veinsInAreaNonAlloc3; l++)
                    {
                        if (BuildTool._tmp_ids[l] != 0 && veinPool3[BuildTool._tmp_ids[l]].id == BuildTool._tmp_ids[l] && veinPool3[BuildTool._tmp_ids[l]].type == EVeinType.Oil)
                        {
                            Vector3 pos2 = veinPool3[BuildTool._tmp_ids[l]].pos;
                            Vector3 vector4 = pos2 - vector2;
                            float d = Vector3.Dot(vector3, vector4);
                            float sqrMagnitude = (vector4 - vector3 * d).sqrMagnitude;
                            if (sqrMagnitude < num3)
                            {
                                num3 = sqrMagnitude;
                                num2 = BuildTool._tmp_ids[l];
                                pos = pos2;
                            }
                        }
                    }
                    if (num2 == 0)
                    {
                        //buildPreview.condition = EBuildCondition.NeedResource;
                        continue;//goto IL_2D8D;
                    }
                    prebuildData2.parameters[0] = num2;
                    prebuildData2.paramCount = 1;
                    prebuildData2.ArrangeParametersArray();
                    buildPreview.parameters = prebuildData2.parameters;
                    buildPreview.paramCount = prebuildData2.paramCount;
                    /*
                    Vector3 vector5 = this.factory.planet.aux.Snap(pos, true);
                    vector = (lPose.position = (buildPreview.lpos2 = (buildPreview.lpos = vector5)));
                    quaternion = (lPose.rotation = (buildPreview.lrot2 = (buildPreview.lrot = Maths.SphericalRotation(vector5, this.yaw))));
                    forward = lPose.forward;
                    up = lPose.up;
                    */
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                }
            }
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
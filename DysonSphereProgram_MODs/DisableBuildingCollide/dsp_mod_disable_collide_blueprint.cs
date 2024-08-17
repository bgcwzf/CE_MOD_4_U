using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>BuildTool_BlueprintPaste:CheckBuildConditions|BuildTool_BlueprintPaste:CheckBuildConditionsPrestage|BuildTool_Dismantle:DeterminePreviews
//MOD_NEW_METHOD==>DSP_CE_MOD.DisableCollide_BuildTool_BlueprintPaste:new_CheckBuildConditions|DSP_CE_MOD.DisableCollide_BuildTool_BlueprintPaste:new_CheckBuildConditionsPrestage|DSP_CE_MOD.DisableDistanceCheck_BuildTool_Dismantle:new_DeterminePreviews
//MOD_OLD_CALLER==>DSP_CE_MOD.DisableCollide_BuildTool_BlueprintPaste:old_CheckBuildConditions|DSP_CE_MOD.DisableCollide_BuildTool_BlueprintPaste:old_CheckBuildConditionsPrestage|DSP_CE_MOD.DisableDistanceCheck_BuildTool_Dismantle:old_DeterminePreviews
//MOD_DESCRIPTION==>1.蓝图粘贴时的建筑检查  2.不再判断拆除建筑时，机甲与目标建筑之间的距离
namespace DSP_CE_MOD
{
    public class DisableCollide_BuildTool_BlueprintPaste : BuildTool_BlueprintPaste
    {
        // 负责蓝图复制的，有专门的另外一个类：BuildTool_BlueprintCopy
        // Token: 0x06000EFA RID: 3834 RVA: 0x00108FE8 File Offset: 0x001071E8
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool new_CheckBuildConditions()
        {
            for (int j = 0; j < this.bpCursor; j++)
            {
                BuildPreview buildPreview2 = this.bpPool[j];
                Vector3 lpos = buildPreview2.lpos;
                Vector3 lpos2 = buildPreview2.lpos2;
                //Quaternion lrot = buildPreview2.lrot;
                //Quaternion lrot2 = buildPreview2.lrot2;
                Pose pose = new Pose(buildPreview2.lpos, buildPreview2.lrot);
                //Pose pose2 = new Pose(buildPreview2.lpos2, buildPreview2.lrot2);
                Vector3 forward = pose.forward;
                //Vector3 forward2 = pose2.forward;
                Vector3 up = pose.up;
                //Vector3 a = Vector3.Lerp(lpos, lpos2, 0.5f);
                Vector3 forward3 = lpos2 - lpos;
                if (forward3.sqrMagnitude < 0.0001f)
                {
                    //forward3 = Maths.SphericalRotation(lpos, 0f).Forward();
                }
                //Quaternion quaternion = Quaternion.LookRotation(forward3, a.normalized);
                //bool flag6 = buildPreview2.desc.minerType == EMinerType.None && buildPreview2.desc.addonType == EAddonType.None && !buildPreview2.desc.isBelt && !buildPreview2.desc.isSplitter && (!buildPreview2.desc.isPowerNode || buildPreview2.desc.isPowerGen || buildPreview2.desc.isAccumulator || buildPreview2.desc.isPowerExchanger) && !buildPreview2.desc.isStation && !buildPreview2.desc.isSilo && !buildPreview2.desc.multiLevel && !buildPreview2.desc.isPiler && !buildPreview2.desc.isBattleBase;
                if (buildPreview2.desc.isSpraycoster || buildPreview2.desc.isTurret)
                {
                    Vector3 reshapeData = SpraycoaterComponent.GetReshapeData(buildPreview2.lpos, buildPreview2.lrot);
                    if (Mathf.Abs(reshapeData.x) > 0.265f || Mathf.Abs(reshapeData.y) > 0.265f)
                    {
                        //注释掉下面的代码：不再设置 缺少目标的采矿资源的错误状态
                        //buildPreview2.condition = EBuildCondition.TooSkew;this.AddErrorMessage(buildPreview2.condition);
                        continue;//goto IL_4C92;
                    }
                }
                if (buildPreview2.desc.veinMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    PrebuildData prebuildData = default(PrebuildData);
                    prebuildData.isDestroyed = false;
                    int paramCount = 0;
                    if (buildPreview2.desc.isVeinCollector)
                    {
                        Vector3 vector = lpos + forward * -10f;
                        Vector3 rhs = -forward;
                        Vector3 right = pose.right;
                        int veinsInAreaNonAlloc = base.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector, 18f, BuildTool._tmp_ids);
                        prebuildData.InitParametersArray(veinsInAreaNonAlloc);
                        VeinData[] veinPool = this.factory.veinPool;
                        EVeinType eveinType = EVeinType.None;
                        for (int k = 0; k < veinsInAreaNonAlloc; k++)
                        {
                            if (BuildTool._tmp_ids[k] != 0 && veinPool[BuildTool._tmp_ids[k]].id == BuildTool._tmp_ids[k])
                            {
                                if (veinPool[BuildTool._tmp_ids[k]].type != EVeinType.Oil)
                                {
                                    Vector3 lhs = veinPool[BuildTool._tmp_ids[k]].pos - vector;
                                    float sqrMagnitude = lhs.sqrMagnitude;
                                    float num = Mathf.Abs(Vector3.Dot(lhs, rhs));
                                    float num2 = Mathf.Abs(Vector3.Dot(lhs, right));
                                    if (sqrMagnitude <= 100f && num <= 7.75f && num2 <= 6.25f)
                                    {
                                        if (eveinType != veinPool[BuildTool._tmp_ids[k]].type)
                                        {
                                            if (eveinType == EVeinType.None)
                                            {
                                                eveinType = veinPool[BuildTool._tmp_ids[k]].type;
                                            }
                                            else
                                            {
                                                //注释掉下面的代码：不再设置 缺少目标的采矿资源的错误状态
                                                //buildPreview2.condition = EBuildCondition.NeedSingleResource;
                                                //this.AddErrorMessage(buildPreview2.condition);
                                            }
                                        }
                                        prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[k];
                                    }
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
                        Vector3 vector2 = lpos + forward * -1.2f;
                        Vector3 rhs2 = -forward;
                        Vector3 vector3 = up;
                        int veinsInAreaNonAlloc2 = base.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector2, 12f, BuildTool._tmp_ids);
                        prebuildData.InitParametersArray(veinsInAreaNonAlloc2);
                        VeinData[] veinPool2 = this.factory.veinPool;
                        EVeinType eveinType2 = EVeinType.None;
                        for (int l = 0; l < veinsInAreaNonAlloc2; l++)
                        {
                            if (BuildTool._tmp_ids[l] != 0 && veinPool2[BuildTool._tmp_ids[l]].id == BuildTool._tmp_ids[l])
                            {
                                if (veinPool2[BuildTool._tmp_ids[l]].type != EVeinType.Oil)
                                {
                                    Vector3 vector4 = veinPool2[BuildTool._tmp_ids[l]].pos - vector2;
                                    float num3 = Vector3.Dot(vector3, vector4);
                                    vector4 -= vector3 * num3;
                                    float sqrMagnitude2 = vector4.sqrMagnitude;
                                    float num4 = Vector3.Dot(vector4.normalized, rhs2);
                                    if (sqrMagnitude2 <= 60.0625f && num4 >= 0.73f && Mathf.Abs(num3) <= 2f)
                                    {
                                        if (eveinType2 != veinPool2[BuildTool._tmp_ids[l]].type)
                                        {
                                            if (eveinType2 == EVeinType.None)
                                            {
                                                eveinType2 = veinPool2[BuildTool._tmp_ids[l]].type;
                                            }
                                            else
                                            {
                                                //注释掉下面的代码：不再设置 缺少目标的采矿资源的错误状态
                                                //buildPreview2.condition = EBuildCondition.NeedResource;
                                                //this.AddErrorMessage(buildPreview2.condition);
                                            }
                                        }
                                        prebuildData.parameters[paramCount++] = BuildTool._tmp_ids[l];
                                    }
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
                    if (buildPreview2.paramCount > 0 && buildPreview2.desc.isVeinCollector)
                    {
                        if (prebuildData.paramCount > 0)
                        {
                            Array.Resize<int>(ref buildPreview2.parameters, buildPreview2.paramCount + prebuildData.paramCount);
                            Array.Copy(prebuildData.parameters, 0, buildPreview2.parameters, buildPreview2.paramCount, prebuildData.paramCount);
                            buildPreview2.paramCount += prebuildData.paramCount;
                        }
                    }
                    else
                    {
                        buildPreview2.parameters = prebuildData.parameters;
                        buildPreview2.paramCount = prebuildData.paramCount;
                    }
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    if (prebuildData.paramCount == 0)
                    {
                        //注释掉下面的代码：不再设置 缺少目标的采矿资源的错误状态
                        //buildPreview2.condition = EBuildCondition.NeedResource;this.AddErrorMessage(buildPreview2.condition);
                        continue;//goto IL_4C92;
                    }
                }
                else if (buildPreview2.desc.oilMiner)
                {
                    Array.Clear(BuildTool._tmp_ids, 0, BuildTool._tmp_ids.Length);
                    Vector3 vector5 = lpos;
                    Vector3 vector6 = -up;
                    int veinsInAreaNonAlloc3 = base.actionBuild.nearcdLogic.GetVeinsInAreaNonAlloc(vector5, 10f, BuildTool._tmp_ids);
                    PrebuildData prebuildData2 = default(PrebuildData);
                    prebuildData2.isDestroyed = false;
                    prebuildData2.InitParametersArray(veinsInAreaNonAlloc3);
                    VeinData[] veinPool3 = this.factory.veinPool;
                    int num5 = 0;
                    float num6 = 0.1f;
                    for (int m = 0; m < veinsInAreaNonAlloc3; m++)
                    {
                        if (BuildTool._tmp_ids[m] != 0 && veinPool3[BuildTool._tmp_ids[m]].id == BuildTool._tmp_ids[m] && veinPool3[BuildTool._tmp_ids[m]].type == EVeinType.Oil)
                        {
                            Vector3 pos = veinPool3[BuildTool._tmp_ids[m]].pos;
                            Vector3 vector7 = pos - vector5;
                            float d = Vector3.Dot(vector6, vector7);
                            float sqrMagnitude3 = (vector7 - vector6 * d).sqrMagnitude;
                            if (sqrMagnitude3 < num6)
                            {
                                num6 = sqrMagnitude3;
                                num5 = BuildTool._tmp_ids[m];
                            }
                        }
                    }
                    if (num5 == 0)
                    {
                        //buildPreview2.condition = EBuildCondition.NeedResource;
                        //this.AddErrorMessage(buildPreview2.condition);
                        continue;//goto IL_4C92;
                    }
                    prebuildData2.parameters[0] = num5;
                    prebuildData2.paramCount = 1;
                    prebuildData2.ArrangeParametersArray();
                    buildPreview2.parameters = prebuildData2.parameters;
                    buildPreview2.paramCount = prebuildData2.paramCount;
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
    public class DisableDistanceCheck_BuildTool_Dismantle : BuildTool_Dismantle
    {
        //不再判断拆除建筑时，机甲与目标建筑之间的距离
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_DeterminePreviews()
        {
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_DeterminePreviews()
        {
            if (!VFInput.onGUI)
            {
                UICursor.SetCursor(ECursor.Delete);
            }
            base.buildPreviews.Clear();
            if (this.cursorType == 0)
            {
                if (this.castObjectId != 0)
                {
                    ItemProto itemProto = base.GetItemProto(this.castObjectId);
                    Pose objectPose = base.GetObjectPose(this.castObjectId);
                    if (itemProto != null)
                    {
                        PrefabDesc prefabDesc = base.GetPrefabDesc(this.castObjectId);
                        if ((prefabDesc.isInserter && this.filterInserter) || (prefabDesc.isBelt && this.filterBelt) || (!prefabDesc.isInserter && !prefabDesc.isBelt && this.filterFacility))
                        {
                            BuildPreview buildPreview = new BuildPreview();
                            buildPreview.item = itemProto;
                            buildPreview.desc = prefabDesc;
                            buildPreview.lpos = objectPose.position;
                            buildPreview.lrot = objectPose.rotation;
                            buildPreview.lpos2 = objectPose.position;
                            buildPreview.lrot2 = objectPose.rotation;
                            buildPreview.objId = this.castObjectId;
                            if (buildPreview.desc.lodCount > 0 && buildPreview.desc.lodMeshes[0] != null)
                            {
                                buildPreview.needModel = true;
                            }
                            else
                            {
                                buildPreview.needModel = false;
                            }
                            buildPreview.isConnNode = true;
                            if (buildPreview.desc.isInserter)
                            {
                                Pose objectPose2 = base.GetObjectPose2(buildPreview.objId);
                                buildPreview.lpos2 = objectPose2.position;
                                buildPreview.lrot2 = objectPose2.rotation;
                            }
                            PlanetData planetData = base.player.planetData;
                            Vector3 vector = base.player.position;
                            if (planetData.type == EPlanetType.Gas)
                            {
                                vector = vector.normalized;
                                vector *= planetData.realRadius;
                            }
                            /*
                            //不再判断拆除距离
                            if ((buildPreview.lpos - vector).sqrMagnitude > base.player.mecha.buildArea * base.player.mecha.buildArea)
                            {
                                buildPreview.condition = EBuildCondition.OutOfReach;
                                base.actionBuild.model.cursorText = "目标超出范围".Translate();
                                base.actionBuild.model.cursorState = -1;
                            }
                            else
                            */
                            {
                                buildPreview.condition = EBuildCondition.Ok;
                                base.actionBuild.model.cursorText = "拆除".Translate() + buildPreview.item.name + "\r\n" + "连锁拆除提示".Translate();
                            }
                            if (buildPreview.desc.multiLevel || buildPreview.desc.isBattleBase)
                            {
                                bool flag;
                                int num;
                                int num2;
                                this.factory.ReadObjectConn(buildPreview.objId, 15, out flag, out num, out num2);
                                if (num != 0)
                                {
                                    buildPreview.condition = EBuildCondition.Covered;
                                    base.actionBuild.model.cursorText = buildPreview.conditionText;
                                    base.actionBuild.model.cursorState = -1;
                                }
                                else
                                {
                                    this.factory.ReadObjectConn(buildPreview.objId, 13, out flag, out num, out num2);
                                    if (num != 0)
                                    {
                                        buildPreview.condition = EBuildCondition.Covered;
                                        base.actionBuild.model.cursorText = buildPreview.conditionText;
                                        base.actionBuild.model.cursorState = -1;
                                    }
                                }
                            }
                            base.buildPreviews.Add(buildPreview);
                        }
                    }
                }
            }
            else if (this.cursorType == 1)
            {
                Vector4 gratbox = Vector4.zero;
                if (VFInput._cursorPlusKey.onDown)
                {
                    this.cursorSize++;
                }
                if (VFInput._cursorMinusKey.onDown)
                {
                    this.cursorSize--;
                }
                if (this.cursorSize < 1)
                {
                    this.cursorSize = 1;
                }
                else if (this.cursorSize > 11)
                {
                    this.cursorSize = 11;
                }
                if (this.castGround)
                {
                    gratbox = base.actionBuild.planetAux.activeGrid.GratboxByCenterSize(this.castGroundPos, this.cursorSize);
                    bool flag2 = false;
                    base.GetOverlappedObjectsNonAlloc(this.castGroundPos, 1.5f * (float)this.cursorSize, 1.5f * (float)this.cursorSize, true);
                    for (int i = 0; i < BuildTool._overlappedCount; i++)
                    {
                        ItemProto itemProto2 = base.GetItemProto(BuildTool._overlappedIds[i]);
                        PrefabDesc prefabDesc2 = base.GetPrefabDesc(BuildTool._overlappedIds[i]);
                        Pose objectPose3 = base.GetObjectPose(BuildTool._overlappedIds[i]);
                        Pose pose = prefabDesc2.isInserter ? base.GetObjectPose2(BuildTool._overlappedIds[i]) : objectPose3;
                        if ((!BuildTool_Dismantle.showDemolishContainerQuery || !prefabDesc2.isStation) && (base.actionBuild.planetAux.activeGrid.IsPointInGratbox(objectPose3.position, gratbox) || (this.filterInserter && prefabDesc2.isInserter && base.actionBuild.planetAux.activeGrid.IsPointInGratbox(pose.position, gratbox))) && ((prefabDesc2.isInserter && this.filterInserter) || (prefabDesc2.isBelt && this.filterBelt) || (!prefabDesc2.isInserter && !prefabDesc2.isBelt && this.filterFacility)))
                        {
                            BuildPreview buildPreview2 = new BuildPreview();
                            buildPreview2.item = itemProto2;
                            buildPreview2.desc = prefabDesc2;
                            buildPreview2.lpos = objectPose3.position;
                            buildPreview2.lrot = objectPose3.rotation;
                            buildPreview2.lpos2 = objectPose3.position;
                            buildPreview2.lrot2 = objectPose3.rotation;
                            buildPreview2.objId = BuildTool._overlappedIds[i];
                            if (buildPreview2.desc.lodCount > 0 && buildPreview2.desc.lodMeshes[0] != null)
                            {
                                buildPreview2.needModel = true;
                            }
                            else
                            {
                                buildPreview2.needModel = false;
                            }
                            buildPreview2.isConnNode = true;
                            if (prefabDesc2.isInserter)
                            {
                                buildPreview2.lpos2 = pose.position;
                                buildPreview2.lrot2 = pose.rotation;
                            }
                            /*
                            //不再判断拆除距离
                            if ((objectPose3.position - base.player.position).sqrMagnitude > base.player.mecha.buildArea * base.player.mecha.buildArea)
                            {
                                buildPreview2.condition = EBuildCondition.OutOfReach;
                                flag2 = true;
                            }
                            else
                            */
                            {
                                buildPreview2.condition = EBuildCondition.Ok;
                            }
                            if (buildPreview2.desc.multiLevel || buildPreview2.desc.isBattleBase)
                            {
                                bool flag3;
                                int num3;
                                int num4;
                                this.factory.ReadObjectConn(buildPreview2.objId, 15, out flag3, out num3, out num4);
                                if (num3 != 0)
                                {
                                    buildPreview2.condition = EBuildCondition.Covered;
                                }
                                else
                                {
                                    this.factory.ReadObjectConn(buildPreview2.objId, 13, out flag3, out num3, out num4);
                                    if (num3 != 0)
                                    {
                                        buildPreview2.condition = EBuildCondition.Covered;
                                    }
                                }
                            }
                            if (buildPreview2.condition == EBuildCondition.Ok)
                            {
                                base.buildPreviews.Add(buildPreview2);
                            }
                        }
                    }
                    if (!flag2)
                    {
                        base.actionBuild.model.cursorText = string.Concat(new object[]
                        {
                    "拆除".Translate(),
                    " (",
                    base.buildPreviews.Count,
                    ")\r\n",
                    "连锁拆除提示".Translate()
                        });
                        base.actionBuild.model.cursorState = 0;
                    }
                    else
                    {
                        base.actionBuild.model.cursorText = "目标超出范围".Translate();
                        base.actionBuild.model.cursorState = -1;
                    }
                }
            }
            this.DetermineMorePreviews();
        }
    }
}
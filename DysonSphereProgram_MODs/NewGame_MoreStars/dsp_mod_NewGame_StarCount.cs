using System;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices; //for NoInlining
using System.Runtime.InteropServices;
using System.Collections.Generic;
//dsp_mod_NewGame_StarCount
//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//用英文半角"|"分隔多个需要Patch的项。下面的NEW_METHOD和OLD_CALLER类同，并且彼此之间是一一对应的关系
//MOD_PATCH_TARGET==>UIGalaxySelect:OnStarCountSliderValueChange|UIGalaxySelect:UpdateUIDisplay|UniverseGen:CreateGalaxy|SectorModel:CreateGalaxyAstroBuffer
//MOD_NEW_METHOD==>DSP_CE_MOD.UIGalaxySelect_OnStarCountSliderValueChange_Patch:new_OnStarCountSliderValueChange|DSP_CE_MOD.UIGalaxySelect_OnStarCountSliderValueChange_Patch:new_UpdateUIDisplay|DSP_CE_MOD.UniverseGen_CreateGalaxy_Patch:new_CreateGalaxy|DSP_CE_MOD.SectorModel_Patch:new_CreateGalaxyAstroBuffer
//MOD_OLD_CALLER==>DSP_CE_MOD.UIGalaxySelect_OnStarCountSliderValueChange_Patch:old_OnStarCountSliderValueChange|DSP_CE_MOD.UIGalaxySelect_OnStarCountSliderValueChange_Patch:old_UpdateUIDisplay|DSP_CE_MOD.UniverseGen_CreateGalaxy_Patch:old_CreateGalaxy|DSP_CE_MOD.SectorModel_Patch:old_CreateGalaxyAstroBuffer
//MOD_DESCRIPTION==>UTF8新档MOD注入-恒星数量乘以10

/*DSP千星数量的修改，共有3处修改（对主DLL文件）

1.
0x15DFE9 附近有连续的2个 64 64 00 00，这里就是AstrosData这个数组的size，将其修改为 64 64 02 00，即可

本质上，是修改类GalaxData的构造函数
public GalaxyData()
{
	this.astrosData = new AstroData[25700];
	this.astrosFactory = new PlanetFactory[25700];
}
中固定的这个数组的大小25700


2.
搜索 4017220000803F6F43140006，找到的第一个 0x40就是界面上滑块的最大值。由于这里是单字节，所以这里最大只能改到127，即0x1F

UIGalaxySelect:_OnOpen函数中的：
this.gameDesc.SetForNewGame(UniverseGen.algoVersion, this.random.Next(100000000), 64 , 1, 1f);
这里的64，这里就是滑块的最大值

3.
0x203142处开始，有多个 00 64 00 00 (十进制的25600），将其修改为 00 64 02 即可
第三处修改的是：SectorModel:CreateGalaxyAstroBuffer函数中创建数组的大小
*/
namespace DSP_CE_MOD
{
    public class UIGalaxySelect_OnStarCountSliderValueChange_Patch : UIGalaxySelect
    {
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern void OutputDebugStringW(string msg);
        public static int scale_fact = 10; //因为界面上的滑块的最大值，只能是一个1字节的正整数，其取值范围只能是1~127，所以单独为它设置一个放大比例
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_OnStarCountSliderValueChange(float val)
        {
            const int maxValue = 1201;
            int num = (int)(this.starCountSlider.value + 0.1f);
            this.starCountSlider.minValue = 32;
            this.starCountSlider.maxValue = maxValue;
            if (num < 16)
            {
                num = 16;
            }
            else if (num > maxValue)
            {
                num = maxValue;
            }
            try
            {
                int scale = scale_fact;//用户选定恒星数量的倍数。默认这里设置为10倍
                //var f = GetPrivateField("gameDesc");
                var f = PrivateHelper<UIGalaxySelect>.GetPrivateField("gameDesc");

                GameDesc d = (GameDesc)f.GetValue(this);
                if (num/*  *scale  */ != d.starCount)
                {
                    //OutputDebugStringW("555");
                    d.starCount = num;// num * scale;
                    this.SetStarmapGalaxy();
                }
                //OutputDebugStringW("777");
            }
            catch (Exception ex)
            {
                OutputDebugStringW(string.Format("UIGalaxySelect_OnStarCountSliderValueChange_Patch Exception: {0}\n{1}", ex.Message, ex.StackTrace));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_OnStarCountSliderValueChange(float val) {}

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_UpdateUIDisplay(GalaxyData galaxy) { }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_UpdateUIDisplay(GalaxyData galaxy)
        {
            var f = PrivateHelper<UIGalaxySelect>.GetPrivateField("gameDesc");
            GameDesc d = (GameDesc)f.GetValue(this);
            try
            {
                this.seedInput.text = galaxy.seed.ToString("0000 0000");
                this.starCountSlider.value = (float)galaxy.starCount;
                this.starCountText.text = galaxy.starCount.ToString();
                int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            int num10 = 0;
            foreach (StarData starData in galaxy.stars)
            {
                if (starData.type == EStarType.MainSeqStar || starData.type == EStarType.GiantStar)
                {
                    if (starData.spectr == ESpectrType.M)
                    {
                        num++;
                    }
                    else if (starData.spectr == ESpectrType.K)
                    {
                        num2++;
                    }
                    else if (starData.spectr == ESpectrType.G)
                    {
                        num3++;
                    }
                    else if (starData.spectr == ESpectrType.F)
                    {
                        num4++;
                    }
                    else if (starData.spectr == ESpectrType.A)
                    {
                        num5++;
                    }
                    else if (starData.spectr == ESpectrType.B)
                    {
                        num6++;
                    }
                    else if (starData.spectr == ESpectrType.O)
                    {
                        num7++;
                    }
                }
                else if (starData.type == EStarType.NeutronStar)
                {
                    num8++;
                }
                else if (starData.type == EStarType.WhiteDwarf)
                {
                    num9++;
                }
                else if (starData.type == EStarType.BlackHole)
                {
                    num10++;
                }
                }
               // OutputDebugStringW("000");
                this.mCountText.text = num.ToString();
            this.kCountText.text = num2.ToString();
            this.gCountText.text = num3.ToString();
            this.fCountText.text = num4.ToString();
            this.aCountText.text = num5.ToString();
            this.bCountText.text = num6.ToString();
             //   OutputDebugStringW("111");
            this.oCountText.text = num7.ToString();
              //  OutputDebugStringW("222");
            this.nCountText.text = num8.ToString();
             //  OutputDebugStringW("333");
            this.wdCountText.text = num9.ToString();
              //  OutputDebugStringW("444");
            this.bhCountText.text = num10.ToString();
              //  OutputDebugStringW("555");
                this.sandboxToggle.isOn = d.isSandboxMode;
             //   OutputDebugStringW("666");
            }
            catch (Exception ex)
            {
                OutputDebugStringW(string.Format("UIGalaxySelect_Patch:UpdateUIDisplay Exception: {0}\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }


    public static class UniverseGen_CreateGalaxy_Patch
    {
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern void OutputDebugStringW(string msg);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static GalaxyData old_CreateGalaxy(GameDesc gameDesc) { return null; }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static GalaxyData new_CreateGalaxy(GameDesc gameDesc)
        {
            int galaxyAlgo = gameDesc.galaxyAlgo;
            int galaxySeed = gameDesc.galaxySeed;
            int num = gameDesc.starCount;
            if (galaxyAlgo < 20200101 || galaxyAlgo > 20591231)
            {
                throw new Exception("Wrong version of unigen algorithm!");
            }
            PlanetGen.gasCoef = (gameDesc.isRareResource ? 0.8f : 1f);
            DotNet35Random dotNet35Random = new DotNet35Random(galaxySeed);

            Type UniverseGenClsType = typeof(UniverseGen);
            //var f = PrivateHelper<UniverseGen>.GetPrivateField("gameDesc");
            //num = UniverseGen.GenerateTempPoses(dotNet35Random.Next(), num, 4, 2.0, 2.3, 3.5, 0.18);
            // 获取私有静态方法
            MethodInfo methodInfo = UniverseGenClsType.GetMethod("GenerateTempPoses", BindingFlags.NonPublic | BindingFlags.Static);
            if (methodInfo != null)
            {
                object[] params1 = { dotNet35Random.Next(), num, 4, 2.0, 2.3, 3.5, 0.18 };
                num = (int)methodInfo.Invoke(null, params1); // 调用私有静态方法
            }
            List<VectorLF3> UniverseGen_tmp_poses = null;
            var fdd = UniverseGenClsType.GetField("tmp_poses", BindingFlags.NonPublic | BindingFlags.Static);
            if (fdd != null)
            {
                UniverseGen_tmp_poses = (List<VectorLF3>)fdd.GetValue(null);
            }


            GalaxyData galaxyData = new GalaxyData();
            galaxyData.seed = galaxySeed;
            galaxyData.starCount = num;
            galaxyData.stars = new StarData[num];


            Assert.Positive(num);
            if (num <= 0)
            {
                return galaxyData;
            }
            float num2 = (float)dotNet35Random.NextDouble();
            float num3 = (float)dotNet35Random.NextDouble();
            float num4 = (float)dotNet35Random.NextDouble();
            float num5 = (float)dotNet35Random.NextDouble();
            int num6 = Mathf.CeilToInt(0.01f * (float)num + num2 * 0.3f);
            int num7 = Mathf.CeilToInt(0.01f * (float)num + num3 * 0.3f);
            int num8 = Mathf.CeilToInt(0.016f * (float)num + num4 * 0.4f);
            int num9 = Mathf.CeilToInt(0.013f * (float)num + num5 * 1.4f);
            int num10 = num - num6;
            int num11 = num10 - num7;
            int num12 = num11 - num8;
            int num13 = (num12 - 1) / num9;
            int num14 = num13 / 2;
            for (int i = 0; i < num; i++)
            {
                int seed = dotNet35Random.Next();
                if (i == 0)
                {
                    galaxyData.stars[i] = StarGen.CreateBirthStar(galaxyData, gameDesc, seed);
                }
                else
                {
                    ESpectrType needSpectr = ESpectrType.X;
                    if (i == 3)
                    {
                        needSpectr = ESpectrType.M;
                    }
                    else if (i == num12 - 1)
                    {
                        needSpectr = ESpectrType.O;
                    }
                    EStarType needtype = EStarType.MainSeqStar;
                    if (i % num13 == num14)
                    {
                        needtype = EStarType.GiantStar;
                    }
                    if (i >= num10)
                    {
                        needtype = EStarType.BlackHole;
                    }
                    else if (i >= num11)
                    {
                        needtype = EStarType.NeutronStar;
                    }
                    else if (i >= num12)
                    {
                        needtype = EStarType.WhiteDwarf;
                    }

                    galaxyData.stars[i] = StarGen.CreateStar(galaxyData, UniverseGen_tmp_poses[i], gameDesc, i + 1, seed, needtype, needSpectr);
                }
            }
            AstroData[] astrosData = galaxyData.astrosData;
            /*if (astrosData.Length < 30000)
            {
                // 扩容数组
                var tmp = new AstroData[64 * 100 * 4 * 10/*UIGalaxySelect_OnStarCountSliderValueChange_Patch.scale_fact];
                Array.Copy(galaxyData.astrosData, tmp, astrosData.Length);
                astrosData=galaxyData.astrosData = tmp;
            }*/
            StarData[] stars = galaxyData.stars;
            for (int j = 0; j < galaxyData.astrosData.Length; j++)
            {
                astrosData[j].uRot.w = 1f;
                astrosData[j].uRotNext.w = 1f;
            }
            //OutputDebugStringW(string.Format("UniverseGen_CreateGalaxy---4::astrosData.Length[{0}]", astrosData.Length));
            for (int k = 0; k < num; k++)
            {
                StarGen.CreateStarPlanets(galaxyData, stars[k], gameDesc);
                //OutputDebugStringW("UniverseGen_CreateGalaxy---4.1");
                int astroId = stars[k].astroId;
                //OutputDebugStringW(string.Format("UniverseGen_CreateGalaxy---4.2::astroId[{0}]", astroId));
                astrosData[astroId].id = astroId;
                //OutputDebugStringW("UniverseGen_CreateGalaxy---4.3");
                astrosData[astroId].type = EAstroType.Star;
                astrosData[astroId].uPos = (astrosData[astroId].uPosNext = stars[k].uPosition);
                astrosData[astroId].uRot = (astrosData[astroId].uRotNext = Quaternion.identity);
                astrosData[astroId].uRadius = stars[k].physicsRadius;
            }
            //OutputDebugStringW("UniverseGen_CreateGalaxy---5");
            galaxyData.UpdatePoses(0.0);
            //OutputDebugStringW("UniverseGen_CreateGalaxy---5.1");
            galaxyData.birthPlanetId = 0;
            if (num > 0)
            {
                //OutputDebugStringW("UniverseGen_CreateGalaxy---5.2");
                StarData starData = stars[0];
                for (int l = 0; l < starData.planetCount; l++)
                {
                    PlanetData planetData = starData.planets[l];
                    ThemeProto themeProto = LDB.themes.Select(planetData.theme);
                    if (themeProto != null && themeProto.Distribute == EThemeDistribute.Birth)
                    {
                        galaxyData.birthPlanetId = planetData.id;
                        galaxyData.birthStarId = starData.id;
                        break;
                    }
                }
            }
            //OutputDebugStringW("UniverseGen_CreateGalaxy---6");
            Assert.Positive(galaxyData.birthPlanetId);
            UniverseGen.CreateGalaxyStarGraph(galaxyData);
            //OutputDebugStringW("UniverseGen_CreateGalaxy---Done");
            PlanetGen.gasCoef = 1f;
            return galaxyData;
        }
    }
    public class SectorModel_Patch : SectorModel
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_CreateGalaxyAstroBuffer() { }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_CreateGalaxyAstroBuffer()
        {
            const int mycustom_size = 0x26464;
            this.galaxyAstroArr = new AstroPoseR[mycustom_size];
            this.galaxyAstroBuffer = new ComputeBuffer(mycustom_size, 32, ComputeBufferType.Default);
            for (int i = 0; i < this.galaxyAstroArr.Length; i++)
            {
                this.galaxyAstroArr[i].rpos.x = 0f;
                this.galaxyAstroArr[i].rpos.y = 0f;
                this.galaxyAstroArr[i].rpos.z = 0f;
                this.galaxyAstroArr[i].rrot.x = 0f;
                this.galaxyAstroArr[i].rrot.y = 0f;
                this.galaxyAstroArr[i].rrot.z = 0f;
                this.galaxyAstroArr[i].rrot.w = 1f;
                this.galaxyAstroArr[i].radius = 0f;
            }
            this.starmapGalaxyAstroArr = new AstroPoseR[mycustom_size];
            this.starmapGalaxyAstroBuffer = new ComputeBuffer(mycustom_size, 32, ComputeBufferType.Default);
            Array.Copy(this.galaxyAstroArr, this.starmapGalaxyAstroArr, this.galaxyAstroArr.Length);
        }
    }
}
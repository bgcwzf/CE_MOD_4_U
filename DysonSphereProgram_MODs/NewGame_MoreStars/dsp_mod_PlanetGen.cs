using System;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices; //for NoInlining
using System.Runtime.InteropServices;
using System.Collections.Generic;
//注意：所有包含了中文字符的cs文件，需要以GB2312编码方式保存
//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>PlanetGen:CreatePlanet
//MOD_NEW_METHOD==>DSP_CE_MOD.PlanetGen_Patch:new_CreatePlanet
//MOD_OLD_CALLER==>DSP_CE_MOD.PlanetGen_Patch:old_CreatePlanet
//MOD_DESCRIPTION==>用于处理开局时，生成行星时，对行星进行一些修改（一般用不到）

//以下这几个是被注释掉的项
//_PATCH_TARGET==>PlanetGen:CreatePlanet|PlanetRawData:InitModData|PlanetData:UpdateRuntimePose
//_NEW_METHOD==>DSP_CE_MOD.PlanetGen_Patch:new_CreatePlanet|DSP_CE_MOD.PlanetRawData_Patch:new_InitModData|DSP_CE_MOD.PlanetData_UpdateRuntimePose_Patch:new_UpdateRuntimePose
//_OLD_CALLER==>DSP_CE_MOD.PlanetGen_Patch:old_CreatePlanet|DSP_CE_MOD.PlanetRawData_Patch:old_InitModData|DSP_CE_MOD.PlanetData_UpdateRuntimePose_Patch:old_UpdateRuntimePose
namespace DSP_CE_MOD
{
    public static class PlanetGen_Patch
    {
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern void OutputDebugStringW(string msg);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PlanetData old_CreatePlanet(GalaxyData galaxy, StarData star, int[] themeIds, int index, int orbitAround, int orbitIndex, int number, bool gasGiant, int info_seed, int gen_seed) { return null; }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static PlanetData new_CreatePlanet(GalaxyData galaxy, StarData star, int[] themeIds, int index, int orbitAround, int orbitIndex, int number, bool gasGiant, int info_seed, int gen_seed)
        {
            PlanetData planetData = new PlanetData();

            OutputDebugStringW(string.Format("CreatePlanet for star[{0}],planet_index[{1}],orbitIndex{2}",star.id,index,orbitIndex));

            DotNet35Random dotNet35Random = new DotNet35Random(info_seed);
            planetData.index = index;
            planetData.galaxy = star.galaxy;
            planetData.star = star;
            planetData.seed = gen_seed;
            planetData.infoSeed = info_seed;
            planetData.orbitAround = orbitAround;
            planetData.orbitIndex = orbitIndex;
            planetData.number = number;
            planetData.id = star.astroId + index + 1;
            StarData[] stars = galaxy.stars;
            int num = 0;
            for (int i = 0; i < star.index; i++)
            {
                num += stars[i].planetCount;
            }
            num += index;
            if (orbitAround > 0)
            {
                int j = 0;
                while (j < star.planetCount)
                {
                    if (orbitAround == star.planets[j].number && star.planets[j].orbitAround == 0)
                    {
                        planetData.orbitAroundPlanet = star.planets[j];
                        if (orbitIndex > 1)
                        {
                            planetData.orbitAroundPlanet.singularity |= EPlanetSingularity.MultipleSatellites;
                            break;
                        }
                        break;
                    }
                    else
                    {
                        j++;
                    }
                }
                Assert.NotNull(planetData.orbitAroundPlanet);
            }
            string str;
            if (star.planetCount <= 20)
            {
                str = NameGen.roman[index + 1];
            }
            else
            {
                str = (index + 1).ToString();
            }

            planetData.name = star.name + " " + str + "号星".Translate();
            double num2 = dotNet35Random.NextDouble();
            double num3 = dotNet35Random.NextDouble();
            double num4 = dotNet35Random.NextDouble();
            double num5 = dotNet35Random.NextDouble();
            double num6 = dotNet35Random.NextDouble();
            double num7 = dotNet35Random.NextDouble();
            double num8 = dotNet35Random.NextDouble();
            double num9 = dotNet35Random.NextDouble();
            double num10 = dotNet35Random.NextDouble();
            double num11 = dotNet35Random.NextDouble();
            double num12 = dotNet35Random.NextDouble();
            double num13 = dotNet35Random.NextDouble();
            double rand = dotNet35Random.NextDouble();
            double num14 = dotNet35Random.NextDouble();
            double rand2 = dotNet35Random.NextDouble();
            double rand3 = dotNet35Random.NextDouble();
            double rand4 = dotNet35Random.NextDouble();
            int theme_seed = dotNet35Random.Next();
            float num15 = Mathf.Pow(1.2f, (float)(num2 * (num3 - 0.5) * 0.5));
            float num16;
            if (orbitAround == 0)
            {
                num16 = StarGen.orbitRadius[orbitIndex] * star.orbitScaler;
                float num17 = (num15 - 1f) / Mathf.Max(1f, num16) + 1f;
                num16 *= num17;
            }
            else
            {
                num16 = (float)((double)((1600f * (float)orbitIndex + 200f) * Mathf.Pow(star.orbitScaler, 0.3f) * Mathf.Lerp(num15, 1f, 0.5f) + planetData.orbitAroundPlanet.realRadius) / 40000.0);
            }
            planetData.orbitRadius = num16;
            planetData.orbitInclination = (float)(num4 * 16.0 - 8.0);
            if (orbitAround > 0)
            {
                planetData.orbitInclination *= 2.2f;
            }
            planetData.orbitLongitude = (float)(num5 * 360.0);
            if (star.type >= EStarType.NeutronStar)
            {
                if (planetData.orbitInclination > 0f)
                {
                    planetData.orbitInclination += 3f;
                }
                else
                {
                    planetData.orbitInclination -= 3f;
                }
            }
            if (planetData.orbitAroundPlanet == null)
            {
                planetData.orbitalPeriod = Math.Sqrt(39.47841760435743 * (double)num16 * (double)num16 * (double)num16 / (1.3538551990520382E-06 * (double)star.mass));
            }
            else
            {
                planetData.orbitalPeriod = Math.Sqrt(39.47841760435743 * (double)num16 * (double)num16 * (double)num16 / 1.0830842106853677E-08);
            }
            planetData.orbitPhase = (float)(num6 * 360.0);
            if (num14 < 0.03999999910593033)
            {
                planetData.obliquity = (float)(num7 * (num8 - 0.5) * 39.9);
                if (planetData.obliquity < 0f)
                {
                    planetData.obliquity -= 70f;
                }
                else
                {
                    planetData.obliquity += 70f;
                }
                planetData.singularity |= EPlanetSingularity.LaySide;
            }
            else if (num14 < 0.10000000149011612)
            {
                planetData.obliquity = (float)(num7 * (num8 - 0.5) * 80.0);
                if (planetData.obliquity < 0f)
                {
                    planetData.obliquity -= 30f;
                }
                else
                {
                    planetData.obliquity += 30f;
                }
            }
            else
            {
                planetData.obliquity = (float)(num7 * (num8 - 0.5) * 60.0);
            }
            planetData.rotationPeriod = (num9 * num10 * 1000.0 + 400.0) * (double)((orbitAround == 0) ? Mathf.Pow(num16, 0.25f) : 1f) * (double)(gasGiant ? 0.2f : 1f);
            if (!gasGiant)
            {
                if (star.type == EStarType.WhiteDwarf)
                {
                    planetData.rotationPeriod *= 0.5;
                }
                else if (star.type == EStarType.NeutronStar)
                {
                    planetData.rotationPeriod *= 0.20000000298023224;
                }
                else if (star.type == EStarType.BlackHole)
                {
                    planetData.rotationPeriod *= 0.15000000596046448;
                }
            }
            planetData.rotationPhase = (float)(num11 * 360.0);
            planetData.sunDistance = ((orbitAround == 0) ? planetData.orbitRadius : planetData.orbitAroundPlanet.orbitRadius);
            planetData.scale = 1f;
            double num18 = (orbitAround == 0) ? planetData.orbitalPeriod : planetData.orbitAroundPlanet.orbitalPeriod;
            planetData.rotationPeriod = 1.0 / (1.0 / num18 + 1.0 / planetData.rotationPeriod);
            if (orbitAround == 0 && orbitIndex <= 4 && !gasGiant)
            {
                if (num14 > 0.9599999785423279)
                {
                    planetData.obliquity *= 0.01f;
                    planetData.rotationPeriod = planetData.orbitalPeriod;
                    planetData.singularity |= EPlanetSingularity.TidalLocked;
                }
                else if (num14 > 0.9300000071525574)
                {
                    planetData.obliquity *= 0.1f;
                    planetData.rotationPeriod = planetData.orbitalPeriod * 0.5;
                    planetData.singularity |= EPlanetSingularity.TidalLocked2;
                }
                else if (num14 > 0.8999999761581421)
                {
                    planetData.obliquity *= 0.2f;
                    planetData.rotationPeriod = planetData.orbitalPeriod * 0.25;
                    planetData.singularity |= EPlanetSingularity.TidalLocked4;
                }
            }
            if (num14 > 0.85 && num14 <= 0.9)
            {
                planetData.rotationPeriod = -planetData.rotationPeriod;
                planetData.singularity |= EPlanetSingularity.ClockwiseRotate;
            }
            planetData.runtimeOrbitRotation = Quaternion.AngleAxis(planetData.orbitLongitude, Vector3.up) * Quaternion.AngleAxis(planetData.orbitInclination, Vector3.forward);
            if (planetData.orbitAroundPlanet != null)
            {
                planetData.runtimeOrbitRotation = planetData.orbitAroundPlanet.runtimeOrbitRotation * planetData.runtimeOrbitRotation;
            }
            planetData.runtimeSystemRotation = planetData.runtimeOrbitRotation * Quaternion.AngleAxis(planetData.obliquity, Vector3.forward);


            float habitableRadius = star.habitableRadius;
            if (gasGiant)
            {
                planetData.type = EPlanetType.Gas;
                planetData.radius = 80f;
                planetData.scale = 10f;
                planetData.habitableBias = 100f;
            }
            else
            {
                float num19 = Mathf.Ceil((float)star.galaxy.starCount * 0.29f);
                if (num19 < 11f)
                {
                    num19 = 11f;
                }
                float num20 = num19 - (float)star.galaxy.habitableCount;
                float num21 = (float)(star.galaxy.starCount - star.index);
                float sunDistance = planetData.sunDistance;
                float num22 = 1000f;
                float num23 = 1000f;
                if (habitableRadius > 0f && sunDistance > 0f)
                {
                    num23 = sunDistance / habitableRadius;
                    num22 = Mathf.Abs(Mathf.Log(num23));
                }
                float num24 = Mathf.Clamp(Mathf.Sqrt(habitableRadius), 1f, 2f) - 0.04f;
                float num25 = num20 / num21;
                num25 = Mathf.Lerp(num25, 0.35f, 0.5f);
                num25 = Mathf.Clamp(num25, 0.08f, 0.8f);
                planetData.habitableBias = num22 * num24;
                planetData.temperatureBias = 1.2f / (num23 + 0.2f) - 1f;
                float num26 = Mathf.Clamp01(planetData.habitableBias / num25);
                float p = num25 * 10f;
                num26 = Mathf.Pow(num26, p);
                if ((num12 > (double)num26 && star.index > 0) || (planetData.orbitAround > 0 && planetData.orbitIndex == 1 && star.index == 0))
                {
                    planetData.type = EPlanetType.Ocean;
                    star.galaxy.habitableCount++;
                }
                else if (num23 < 0.833333f)
                {
                    float num27 = Mathf.Max(0.15f, num23 * 2.5f - 0.85f);
                    if (num13 < (double)num27)
                    {
                        planetData.type = EPlanetType.Desert;
                    }
                    else
                    {
                        planetData.type = EPlanetType.Vocano;
                    }
                }
                else if (num23 < 1.2f)
                {
                    planetData.type = EPlanetType.Desert;
                }
                else
                {
                    float num28 = 0.9f / num23 - 0.1f;
                    if (num13 < (double)num28)
                    {
                        planetData.type = EPlanetType.Desert;
                    }
                    else
                    {
                        planetData.type = EPlanetType.Ice;
                    }
                }
                planetData.radius = 200f;
            }
            if (planetData.type != EPlanetType.Gas && planetData.type != EPlanetType.None)
            {
                planetData.precision = 200;
                planetData.segment = 5;
            }
            else
            {
                planetData.precision = 64;
                planetData.segment = 2;
            }
            planetData.luminosity = Mathf.Pow(planetData.star.lightBalanceRadius / (planetData.sunDistance + 0.01f), 0.6f);
            if (planetData.luminosity > 1f)
            {
                planetData.luminosity = Mathf.Log(planetData.luminosity) + 1f;
                planetData.luminosity = Mathf.Log(planetData.luminosity) + 1f;
                planetData.luminosity = Mathf.Log(planetData.luminosity) + 1f;
            }
            planetData.luminosity = Mathf.Round(planetData.luminosity * 100f) / 100f;

            PlanetGen.SetPlanetTheme(planetData, themeIds, rand, rand2, rand3, rand4, theme_seed);

            //OutputDebugStringW(string.Format("CreatePlanet---4::planetData.id[{0}],astrosData.Length[{1}]", planetData.id,star.galaxy.astrosData.Length));
            if (star.galaxy.astrosData.Length < 30000)
            {
                // 扩容数组
                var tmp= new AstroData[64*100*4* 10/*UIGalaxySelect_OnStarCountSliderValueChange_Patch.scale_fact*/];
                Array.Copy(star.galaxy.astrosData, tmp, star.galaxy.astrosData.Length);
                star.galaxy.astrosData=tmp;
            }
            star.galaxy.astrosData[planetData.id].id = planetData.id;
            //OutputDebugStringW("CreatePlanet---5");
            star.galaxy.astrosData[planetData.id].type = EAstroType.Planet;
            star.galaxy.astrosData[planetData.id].parentId = planetData.star.astroId;
            star.galaxy.astrosData[planetData.id].uRadius = planetData.realRadius;
            //OutputDebugStringW("CreatePlanet---6");
            return planetData;
        }
    }
    /*
    public class PlanetData_UpdateRuntimePose_Patch : PlanetData
    {
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern void OutputDebugStringW(string msg);
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void old_UpdateRuntimePose(double refModData) {}

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void new_UpdateRuntimePose(double time)
        {
            double num = time / this.orbitalPeriod + (double)this.orbitPhase / 360.0;
            int num2 = (int)(num + 0.1);
            num -= (double)num2;
            this.runtimeOrbitPhase = (float)num * 360f;
            num *= 6.283185307179586;
            double num3 = time / this.rotationPeriod + (double)this.rotationPhase / 360.0;
            int num4 = (int)(num3 + 0.1);
            num3 = (num3 - (double)num4) * 360.0;
            this.runtimeRotationPhase = (float)num3;
            VectorLF3 vectorLF = Maths.QRotateLF(this.runtimeOrbitRotation, new VectorLF3((float)Math.Cos(num) * this.orbitRadius, 0f, (float)Math.Sin(num) * this.orbitRadius));
            if (this.orbitAroundPlanet != null)
            {
                vectorLF.x += this.orbitAroundPlanet.runtimePosition.x;
                vectorLF.y += this.orbitAroundPlanet.runtimePosition.y;
                vectorLF.z += this.orbitAroundPlanet.runtimePosition.z;
            }
            this.runtimePosition = vectorLF;
            this.runtimeRotation = this.runtimeSystemRotation * Quaternion.AngleAxis((float)num3, Vector3.down);
            this.uPosition.x = this.star.uPosition.x + vectorLF.x * 40000.0;
            this.uPosition.y = this.star.uPosition.y + vectorLF.y * 40000.0;
            this.uPosition.z = this.star.uPosition.z + vectorLF.z * 40000.0;
            this.runtimeLocalSunDirection = Maths.QInvRotate(this.runtimeRotation, -vectorLF);
            double num5 = time + 0.016666666666666666;
            double num6 = num5 / this.orbitalPeriod + (double)this.orbitPhase / 360.0;
            int num7 = (int)(num6 + 0.1);
            num6 -= (double)num7;
            num6 *= 6.283185307179586;
            double num8 = num5 / this.rotationPeriod + (double)this.rotationPhase / 360.0;
            int num9 = (int)(num8 + 0.1);
            num8 = (num8 - (double)num9) * 360.0;
            VectorLF3 vectorLF2 = Maths.QRotateLF(this.runtimeOrbitRotation, new VectorLF3((float)Math.Cos(num6) * this.orbitRadius, 0f, (float)Math.Sin(num6) * this.orbitRadius));
            if (this.orbitAroundPlanet != null)
            {
                vectorLF2.x += this.orbitAroundPlanet.runtimePositionNext.x;
                vectorLF2.y += this.orbitAroundPlanet.runtimePositionNext.y;
                vectorLF2.z += this.orbitAroundPlanet.runtimePositionNext.z;
            }
            this.runtimePositionNext = vectorLF2;
            this.runtimeRotationNext = this.runtimeSystemRotation * Quaternion.AngleAxis((float)num8, Vector3.down);
            this.uPositionNext.x = this.star.uPosition.x + vectorLF2.x * 40000.0;
            this.uPositionNext.y = this.star.uPosition.y + vectorLF2.y * 40000.0;
            this.uPositionNext.z = this.star.uPosition.z + vectorLF2.z * 40000.0;
            OutputDebugStringW(string.Format("PlanetData_UpdateRuntimePose---4::astrosData.Length[{0}],this.id[{1}]", this.galaxy.astrosData.Length, this.id));
            if (this.galaxy.astrosData.Length < 30000)
            {
                // 扩容数组
                var tmp = new AstroData[64 * 100 * 4 * 10/*UIGalaxySelect_OnStarCountSliderValueChange_Patch.scale_fact*/];
                Array.Copy(this.galaxy.astrosData, tmp, this.galaxy.astrosData.Length);
                this.galaxy.astrosData = tmp;
            }
            this.galaxy.astrosData[this.id].uPos = this.uPosition;
            this.galaxy.astrosData[this.id].uRot = this.runtimeRotation;
            this.galaxy.astrosData[this.id].uPosNext = this.uPositionNext;
            this.galaxy.astrosData[this.id].uRotNext = this.runtimeRotationNext;
            this.galaxy.astrosFactory[this.id] = this.factory;
            OutputDebugStringW("PlanetData_UpdateRuntimePose---6");
        }
    }
    public class PlanetRawData_Patch : PlanetRawData
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public PlanetRawData_Patch(int _precision) : base(_precision) { }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public byte[] old_InitModData(byte[] refModData) { return null; }
        // Token: 0x06000F23 RID: 3875 RVA: 0x000D844D File Offset: 0x000D664D
        [MethodImpl(MethodImplOptions.NoInlining)]
        public byte[] new_InitModData(byte[] refModData)
        {
            this.modData = new byte[this.dataLength / 2 + 91000];
            return this.modData;
        }
    }
    */
}
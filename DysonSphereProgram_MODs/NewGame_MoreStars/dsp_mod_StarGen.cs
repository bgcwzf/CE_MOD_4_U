using System;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices; //for NoInlining
using System.Runtime.InteropServices;
using System.Collections.Generic;
//注意：所有包含了中文字符的cs文件，需要以GB2312编码方式保存
//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>StarGen:CreateStarPlanets
//MOD_NEW_METHOD==>DSP_CE_MOD.StarGen_CreateStarPlanets_Patch:new_CreateStarPlanets
//MOD_OLD_CALLER==>DSP_CE_MOD.StarGen_CreateStarPlanets_Patch:old_CreateStarPlanets

//MOD_DESCRIPTION==>用于处理开局时，每个星系多n个行星

namespace DSP_CE_MOD
{
    public static class StarGen_CreateStarPlanets_Patch
    {
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern void OutputDebugStringW(string msg);
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SetHiveOrbitsConditionsTrue()
        {
            for (int i = 0; i < StarGen.hiveOrbitCondition.Length; i++)
            {
                StarGen.hiveOrbitCondition[i] = true;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SetHiveOrbitConditionFalse(int planetOrbitIndex, int orbitAroundOrbitIndex, float sunDistance, int starIndex)
        {
            int num = (orbitAroundOrbitIndex > 0) ? orbitAroundOrbitIndex : planetOrbitIndex;
            int num2 = (orbitAroundOrbitIndex > 0) ? planetOrbitIndex : 0;
            if (num <= 0)
            {
                return;
            }
            if (num >= StarGen.planet2HiveOrbitTable.Length)
            {
                return;
            }
            int num3 = StarGen.planet2HiveOrbitTable[num];
            StarGen.hiveOrbitCondition[num3] = false;
            if (num2 > 0)
            {
                float num4;
                if (starIndex == 0)
                {
                    num4 = 0.041f * (float)num2 + 0.026f + 0.12f;
                }
                else
                {
                    num4 = 0.049f * (float)num2 + 0.026f + 0.13f;
                }
                int num5 = num3 - 1;
                int num6 = num3 + 1;
                if (num5 >= 0 && sunDistance - StarGen.hiveOrbitRadius[num5] < num4)
                {
                    StarGen.hiveOrbitCondition[num5] = false;
                }
                if (num6 < StarGen.hiveOrbitCondition.Length && StarGen.hiveOrbitRadius[num6] - sunDistance < num4)
                {
                    StarGen.hiveOrbitCondition[num6] = false;
                }
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int get_each_star_have_planet_num(DotNet35Random dotNet35Random2)
        {
            const int each_star_have_planet_num = 8;
            return 2+(int)(dotNet35Random2.NextDouble()*each_star_have_planet_num);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void old_CreateStarPlanets(GalaxyData galaxy, StarData star, GameDesc gameDesc) {  }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void new_CreateStarPlanets(GalaxyData galaxy, StarData star, GameDesc gameDesc)
        {
            DotNet35Random dotNet35Random = new DotNet35Random(star.seed);
            dotNet35Random.Next();
            dotNet35Random.Next();
            dotNet35Random.Next();
            DotNet35Random dotNet35Random2 = new DotNet35Random(dotNet35Random.Next());
            double num = dotNet35Random2.NextDouble();
            double num2 = dotNet35Random2.NextDouble();
            double num3 = dotNet35Random2.NextDouble();
            double num4 = dotNet35Random2.NextDouble();
            double num5 = dotNet35Random2.NextDouble();
            double num6 = dotNet35Random2.NextDouble() * 0.2 + 0.9;
            double num7 = dotNet35Random2.NextDouble() * 0.2 + 0.9;
            DotNet35Random dotNet35Random3 = new DotNet35Random(dotNet35Random.Next());
            SetHiveOrbitsConditionsTrue();
            var fd_StarGenPGAS=typeof(StarGen).GetField("pGas", BindingFlags.NonPublic | BindingFlags.Static);
            double[] StarGenPGAS = (double[])fd_StarGenPGAS.GetValue(null);

            //OutputDebugStringW(string.Format("CreateStarPlanets--start-set-planet-count-of-star name[{0}],planet count[{1}]", star.name, star.planetCount));
            if (star.type == EStarType.BlackHole)
            {
                star.planetCount =2;
                star.planets = new PlanetData[star.planetCount];
                int info_seed = dotNet35Random2.Next();
                int gen_seed = dotNet35Random2.Next();
                star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, 2, 1, false, info_seed, gen_seed);
                star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 0, 3, 1, false,dotNet35Random2.Next(),dotNet35Random2.Next());
            }
            else if (star.type == EStarType.NeutronStar)
            {
                star.planetCount = 2;
                star.planets = new PlanetData[star.planetCount];
                int info_seed2 = dotNet35Random2.Next();
                int gen_seed2 = dotNet35Random2.Next();
                star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, 1, 1, false, info_seed2, gen_seed2);
                star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 0, 3, 1, false, dotNet35Random2.Next(),dotNet35Random2.Next());
            }
            else if (star.type == EStarType.WhiteDwarf)
            {
                if (num < 0.699999988079071)
                {
                    star.planetCount = 2;
                    star.planets = new PlanetData[star.planetCount];
                    int info_seed3 = dotNet35Random2.Next();
                    int gen_seed3 = dotNet35Random2.Next();
                    star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, 3, 1, false, info_seed3, gen_seed3);
                    star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds,1, 0,2, 1, false, dotNet35Random2.Next(), dotNet35Random2.Next());
                }
                else
                {
                    star.planetCount = 2;
                    star.planets = new PlanetData[star.planetCount];
                    if (num2 < 0.30000001192092896)
                    {
                        int info_seed4 = dotNet35Random2.Next();
                        int gen_seed4 = dotNet35Random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, 3, 1, false, info_seed4, gen_seed4);
                        info_seed4 = dotNet35Random2.Next();
                        gen_seed4 = dotNet35Random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 0, 4, 2, false, info_seed4, gen_seed4);
                    }
                    else
                    {
                        int info_seed4 = dotNet35Random2.Next();
                        int gen_seed4 = dotNet35Random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, 4, 1, true, info_seed4, gen_seed4);
                        info_seed4 = dotNet35Random2.Next();
                        gen_seed4 = dotNet35Random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 1, 1, 1, false, info_seed4, gen_seed4);
                    }
                }
            }
            else if (star.type == EStarType.GiantStar)
            {
                if (num < 0.30000001192092896)
                {
                    star.planetCount = 2;
                    star.planets = new PlanetData[star.planetCount];
                    int info_seed5 = dotNet35Random2.Next();
                    int gen_seed5 = dotNet35Random2.Next();
                    int orbitIndex = (num3 > 0.5) ? 3 : 2;
                    star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, orbitIndex, 1, false, info_seed5, gen_seed5);
                    star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds,1, 0, orbitIndex+1, 1, false, info_seed5, gen_seed5);
                }
                else if (num < 0.800000011920929)
                {
                    star.planetCount = 2;
                    star.planets = new PlanetData[star.planetCount];
                    if (num2 < 0.25)
                    {
                        int info_seed6 = dotNet35Random2.Next();
                        int gen_seed6 = dotNet35Random2.Next();
                        int orbitIndex2 = (num3 > 0.5) ? 3 : 2;
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, orbitIndex2, 1, false, info_seed6, gen_seed6);
                        info_seed6 = dotNet35Random2.Next();
                        gen_seed6 = dotNet35Random2.Next();
                        orbitIndex2 = ((num3 > 0.5) ? 4 : 3);
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 0, orbitIndex2, 2, false, info_seed6, gen_seed6);
                    }
                    else
                    {
                        int info_seed6 = dotNet35Random2.Next();
                        int gen_seed6 = dotNet35Random2.Next();
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, 3, 1, true, info_seed6, gen_seed6);
                        info_seed6 = dotNet35Random2.Next();
                        gen_seed6 = dotNet35Random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 1, 1, 1, false, info_seed6, gen_seed6);
                    }
                }
                else
                {
                    star.planetCount = 4;
                    star.planets = new PlanetData[star.planetCount];
                    if (num2 < 0.15000000596046448)
                    {
                        int info_seed7 = dotNet35Random2.Next();
                        int gen_seed7 = dotNet35Random2.Next();
                        int orbitIndex3 = (num3 > 0.5) ? 3 : 2;
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, orbitIndex3, 1, false, info_seed7, gen_seed7);
                        info_seed7 = dotNet35Random2.Next();
                        gen_seed7 = dotNet35Random2.Next();
                        orbitIndex3 = ((num3 > 0.5) ? 4 : 3);
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 0, orbitIndex3, 2, false, info_seed7, gen_seed7);
                        info_seed7 = dotNet35Random2.Next();
                        gen_seed7 = dotNet35Random2.Next();
                        orbitIndex3 = ((num3 > 0.5) ? 5 : 4);
                        star.planets[2] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 2, 0, orbitIndex3, 3, false, info_seed7, gen_seed7);
                        star.planets[3] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 3, 0, orbitIndex3+1, 3, false, dotNet35Random2.Next(), dotNet35Random2.Next());
                    }
                    else if (num2 < 0.75)
                    {
                        int info_seed7 = dotNet35Random2.Next();
                        int gen_seed7 = dotNet35Random2.Next();
                        int orbitIndex4 = (num3 > 0.5) ? 3 : 2;
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, orbitIndex4, 1, false, info_seed7, gen_seed7);
                        info_seed7 = dotNet35Random2.Next();
                        gen_seed7 = dotNet35Random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 0, 4, 2, true, info_seed7, gen_seed7);
                        info_seed7 = dotNet35Random2.Next();
                        gen_seed7 = dotNet35Random2.Next();
                        star.planets[2] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 2, 2, 1, 1, false, info_seed7, gen_seed7);
                        star.planets[3] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 3, 2, 5, 1, false, dotNet35Random2.Next(), dotNet35Random2.Next());
                    }
                    else
                    {
                        int info_seed7 = dotNet35Random2.Next();
                        int gen_seed7 = dotNet35Random2.Next();
                        int orbitIndex5 = (num3 > 0.5) ? 4 : 3;
                        star.planets[0] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 0, 0, orbitIndex5, 1, true, info_seed7, gen_seed7);
                        info_seed7 = dotNet35Random2.Next();
                        gen_seed7 = dotNet35Random2.Next();
                        star.planets[1] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 1, 1, 1, 1, false, info_seed7, gen_seed7);
                        info_seed7 = dotNet35Random2.Next();
                        gen_seed7 = dotNet35Random2.Next();
                        star.planets[2] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 2, 1, 2, 2, false, info_seed7, gen_seed7);
                        star.planets[3] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, 3, 1, 3, 2, false, dotNet35Random2.Next(), dotNet35Random2.Next());
                    }
                }
            }
            else
            {
                //normal type of stars
                Array.Clear(StarGenPGAS, 0, StarGenPGAS.Length);
                if (star.index == 0)
                {
                    star.planetCount = 4;
                    StarGenPGAS[0] = 0.0;
                    StarGenPGAS[1] = 0.0;
                    StarGenPGAS[2] = 0.0;
                }
                else if (star.spectr == ESpectrType.M)
                {
                    if (num < 0.1)
                    {
                        star.planetCount = 1;
                    }
                    else if (num < 0.3)
                    {
                        star.planetCount = 2;
                    }
                    else if (num < 0.8)
                    {
                        star.planetCount = 3;
                    }
                    else
                    {
                        star.planetCount = 4;
                    }
                    if (star.planetCount <= 3)
                    {
                        StarGenPGAS[0] = 0.2;
                        StarGenPGAS[1] = 0.2;
                    }
                    else
                    {
                        StarGenPGAS[0] = 0.0;
                        StarGenPGAS[1] = 0.2;
                        StarGenPGAS[2] = 0.3;
                    }
                }
                else if (star.spectr == ESpectrType.K)
                {
                    if (num < 0.1)
                    {
                        star.planetCount = 1;
                    }
                    else if (num < 0.2)
                    {
                        star.planetCount = 2;
                    }
                    else if (num < 0.7)
                    {
                        star.planetCount = 3;
                    }
                    else if (num < 0.95)
                    {
                        star.planetCount = 4;
                    }
                    else
                    {
                        star.planetCount = 5;
                    }
                    if (star.planetCount <= 3)
                    {
                        StarGenPGAS[0] = 0.18;
                        StarGenPGAS[1] = 0.18;
                    }
                    else
                    {
                        StarGenPGAS[0] = 0.0;
                        StarGenPGAS[1] = 0.18;
                        StarGenPGAS[2] = 0.28;
                        StarGenPGAS[3] = 0.28;
                    }
                }
                else if (star.spectr == ESpectrType.G)
                {
                    if (num < 0.4)
                    {
                        star.planetCount = 3;
                    }
                    else if (num < 0.9)
                    {
                        star.planetCount = 4;
                    }
                    else
                    {
                        star.planetCount = 5;
                    }
                    if (star.planetCount <= 3)
                    {
                        StarGenPGAS[0] = 0.18;
                        StarGenPGAS[1] = 0.18;
                    }
                    else
                    {
                        StarGenPGAS[0] = 0.0;
                        StarGenPGAS[1] = 0.2;
                        StarGenPGAS[2] = 0.3;
                        StarGenPGAS[3] = 0.3;
                    }
                }
                else if (star.spectr == ESpectrType.F)
                {
                    if (num < 0.35)
                    {
                        star.planetCount = 3;
                    }
                    else if (num < 0.8)
                    {
                        star.planetCount = 4;
                    }
                    else
                    {
                        star.planetCount = 5;
                    }
                    if (star.planetCount <= 3)
                    {
                        StarGenPGAS[0] = 0.2;
                        StarGenPGAS[1] = 0.2;
                    }
                    else
                    {
                        StarGenPGAS[0] = 0.0;
                        StarGenPGAS[1] = 0.22;
                        StarGenPGAS[2] = 0.31;
                        StarGenPGAS[3] = 0.31;
                    }
                }
                else if (star.spectr == ESpectrType.A)
                {
                    if (num < 0.3)
                    {
                        star.planetCount = 3;
                    }
                    else if (num < 0.75)
                    {
                        star.planetCount = 4;
                    }
                    else
                    {
                        star.planetCount = 5;
                    }
                    if (star.planetCount <= 3)
                    {
                        StarGenPGAS[0] = 0.2;
                        StarGenPGAS[1] = 0.2;
                    }
                    else
                    {
                        StarGenPGAS[0] = 0.1;
                        StarGenPGAS[1] = 0.28;
                        StarGenPGAS[2] = 0.3;
                        StarGenPGAS[3] = 0.35;
                    }
                }
                else if (star.spectr == ESpectrType.B)
                {
                    if (num < 0.3)
                    {
                        star.planetCount = 4;
                    }
                    else if (num < 0.75)
                    {
                        star.planetCount = 5;
                    }
                    else
                    {
                        star.planetCount = 6;
                    }
                    if (star.planetCount <= 3)
                    {
                        StarGenPGAS[0] = 0.2;
                        StarGenPGAS[1] = 0.2;
                    }
                    else
                    {
                        StarGenPGAS[0] = 0.1;
                        StarGenPGAS[1] = 0.22;
                        StarGenPGAS[2] = 0.28;
                        StarGenPGAS[3] = 0.35;
                        StarGenPGAS[4] = 0.35;
                    }
                }
                else if (star.spectr == ESpectrType.O)
                {
                    if (num < 0.5)
                    {
                        star.planetCount = 5;
                    }
                    else
                    {
                        star.planetCount = 6;
                    }
                    StarGenPGAS[0] = 0.1;
                    StarGenPGAS[1] = 0.2;
                    StarGenPGAS[2] = 0.25;
                    StarGenPGAS[3] = 0.3;
                    StarGenPGAS[4] = 0.32;
                    StarGenPGAS[5] = 0.35;
                }
                else
                {
                    star.planetCount = 1;
                }



                //OutputDebugStringW(string.Format("CreateStarPlanets--before-star name[{0}],planet count[{1}]", star.name,star.planetCount));
                star.planetCount *= 3;
                if (star.planetCount > 10) star.planetCount = 10;


                star.planets = new PlanetData[star.planetCount];
                int num8 = 0;
                int num9 = 0;
                int num10 = 0;
                int num11 = 1;
                for (int i = 0; i < star.planetCount; i++)
                {
                    int info_seed8 = dotNet35Random2.Next();
                    int gen_seed8 = dotNet35Random2.Next();
                    double num12 = dotNet35Random2.NextDouble();
                    double num13 = dotNet35Random2.NextDouble();
                    bool flag = false;
                    if (num10 == 0)
                    {
                        num8++;
                        if (i < star.planetCount - 1 && num12 < StarGenPGAS[i])
                        {
                            flag = true;
                            if (num11 < 3)
                            {
                                num11 = 3;
                            }
                        }
                        while (star.index != 0 || num11 != 3)
                        {
                            int num14 = star.planetCount - i;
                            int num15 = 9 - num11;
                            if (num15 <= num14)
                            {
                                goto IL_CB5;
                            }
                            float num16 = (float)num14 / (float)num15;
                            if (num11 > 3)
                            {
                                num16 = Mathf.Lerp(num16, 1f, 0.45f) + 0.01f;
                            }
                            else
                            {
                                num16 = Mathf.Lerp(num16, 1f, 0.15f) + 0.01f;
                            }
                            if (dotNet35Random2.NextDouble() < (double)num16)
                            {
                                goto IL_CB5;
                            }
                            num11++;
                        }
                        flag = true;
                    }
                    else
                    {
                        num9++;
                        flag = false;
                    }
                IL_CB5:
                    star.planets[i] = PlanetGen.CreatePlanet(galaxy, star, gameDesc.savedThemeIds, i, num10, (num10 == 0) ? num11 : num9, (num10 == 0) ? num8 : num9, flag, info_seed8, gen_seed8);
                    num11++;
                    if (flag)
                    {
                        num10 = num8;
                        num9 = 0;
                    }
                    if (num9 >= 1 && num13 < 0.8)
                    {
                        num10 = 0;
                        num9 = 0;
                    }
                }
            }
            //end of normal type of stars

            //OutputDebugStringW(string.Format("CreateStarPlanets-end of normal type of stars-after-star name[{0}],planet count[{1}],star.planets.length[{2}]", star.name, star.planetCount, star.planets.Length));
            int num17 = 0;
            int num18 = 0;
            int num19 = 0;
            for (int j = 0; j < star.planetCount; j++)
            {
                if (star.planets[j].type == EPlanetType.Gas)
                {
                    num17 = star.planets[j].orbitIndex;
                    break;
                }
            }
            //OutputDebugStringW(string.Format("CreateStarPlanets---0---star name[{0}],planet count[{1}]", star.name, star.planetCount));
            for (int k = 0; k < star.planetCount; k++)
            {
                if (star.planets[k].orbitAround == 0)
                {
                    num18 = star.planets[k].orbitIndex;
                }
            }
            //OutputDebugStringW(string.Format("CreateStarPlanets---1---star name[{0}],planet count[{1}]", star.name, star.planetCount));
            if (num17 > 0)
            {
                int num20 = num17 - 1;
                bool flag2 = true;
                for (int l = 0; l < star.planetCount; l++)
                {
                    if (star.planets[l].orbitAround == 0 && star.planets[l].orbitIndex == num17 - 1)
                    {
                        flag2 = false;
                        break;
                    }
                }
                if (flag2 && num4 < 0.2 + (double)num20 * 0.2)
                {
                    num19 = num20;
                }
            }
            //OutputDebugStringW(string.Format("CreateStarPlanets---3---star name[{0}],planet count[{1}]", star.name, star.planetCount));
            int num21;
            if (num5 < 0.2)
            {
                num21 = num18 + 3;
            }
            else if (num5 < 0.4)
            {
                num21 = num18 + 2;
            }
            else if (num5 < 0.8)
            {
                num21 = num18 + 1;
            }
            else
            {
                num21 = 0;
            }
            if (num21 != 0 && num21 < 5)
            {
                num21 = 5;
            }
            star.asterBelt1OrbitIndex = (float)num19;
            star.asterBelt2OrbitIndex = (float)num21;
            //OutputDebugStringW(string.Format("CreateStarPlanets---10---star name[{0}],planet count[{1}],orbitRadius[{2},{3}]", star.name, star.planetCount,num19,num21));
            if (num19 > 0)
            {
                star.asterBelt1Radius = StarGen.orbitRadius[num19] * (float)num6 * star.orbitScaler;
            }
            if (num21 > 0)
            {
                star.asterBelt2Radius = StarGen.orbitRadius[num21] * (float)num7 * star.orbitScaler;
            }
            //OutputDebugStringW(string.Format("CreateStarPlanets---2---star name[{0}],planet count[{1}]", star.name, star.planetCount));
            for (int m = 0; m < star.planetCount; m++)
            {
                PlanetData planetData = star.planets[m];
                int orbitIndex6 = planetData.orbitIndex;
                int orbitAroundOrbitIndex = (planetData.orbitAroundPlanet != null) ? planetData.orbitAroundPlanet.orbitIndex : 0;
                SetHiveOrbitConditionFalse(orbitIndex6, orbitAroundOrbitIndex, planetData.sunDistance / star.orbitScaler, star.index);
            }
            //OutputDebugStringW(string.Format("CreateStarPlanets---3---star name[{0}],planet count[{1}]", star.name, star.planetCount));
            star.hiveAstroOrbits = new AstroOrbitData[8];
            AstroOrbitData[] hiveAstroOrbits = star.hiveAstroOrbits;
            int num22 = 0;
            for (int n = 0; n < StarGen.hiveOrbitCondition.Length; n++)
            {
                if (StarGen.hiveOrbitCondition[n])
                {
                    num22++;
                }
            }
            //OutputDebugStringW(string.Format("CreateStarPlanets---4---star name[{0}],planet count[{1}]", star.name, star.planetCount));
            for (int num23 = 0; num23 < 8; num23++)
            {
                double num24 = dotNet35Random3.NextDouble() * 2.0 - 1.0;
                double num25 = dotNet35Random3.NextDouble();
                double num26 = dotNet35Random3.NextDouble();
                num24 = (double)Math.Sign(num24) * Math.Pow(Math.Abs(num24), 0.7) * 90.0;
                num25 *= 360.0;
                num26 *= 360.0;
                float num27 = 0.3f;
                Assert.Positive(num22);
                if (num22 > 0)
                {
                    int num28 = (star.index != 0) ? 5 : 2;
                    num28 = ((num22 > num28) ? num28 : num22);
                    int num29 = num28 * 100;
                    int num30 = num29 * 100;
                    int num31 = dotNet35Random3.Next(num29);
                    int num32 = num31 * num31 / num30;
                    for (int num33 = 0; num33 < StarGen.hiveOrbitCondition.Length; num33++)
                    {
                        if (StarGen.hiveOrbitCondition[num33])
                        {
                            if (num32 == 0)
                            {
                                num27 = StarGen.hiveOrbitRadius[num33];
                                StarGen.hiveOrbitCondition[num33] = false;
                                num22--;
                                break;
                            }
                            num32--;
                        }
                    }
                }
                //OutputDebugStringW(string.Format("CreateStarPlanets---5.1---star name[{0}],planet count[{1}]", star.name, star.planetCount));
                float num34 = num27 * star.orbitScaler;
                hiveAstroOrbits[num23] = new AstroOrbitData();
                hiveAstroOrbits[num23].orbitRadius = num34;
                hiveAstroOrbits[num23].orbitInclination = (float)num24;
                hiveAstroOrbits[num23].orbitLongitude = (float)num25;
                hiveAstroOrbits[num23].orbitPhase = (float)num26;
                if (gameDesc.creationVersion.Build < 20700)
                {
                    hiveAstroOrbits[num23].orbitalPeriod = Math.Sqrt(39.47841760435743 * (double)num27 * (double)num27 * (double)num27 / (1.3538551990520382E-06 * (double)star.mass));
                }
                else
                {
                    hiveAstroOrbits[num23].orbitalPeriod = Math.Sqrt(39.47841760435743 * (double)num34 * (double)num34 * (double)num34 / (5.415420796208153E-06 * (double)star.mass));
                }
                hiveAstroOrbits[num23].orbitRotation = Quaternion.AngleAxis(hiveAstroOrbits[num23].orbitLongitude, Vector3.up) * Quaternion.AngleAxis(hiveAstroOrbits[num23].orbitInclination, Vector3.forward);
                hiveAstroOrbits[num23].orbitNormal = Maths.QRotateLF(hiveAstroOrbits[num23].orbitRotation, new VectorLF3(0f, 1f, 0f)).normalized;
                //OutputDebugStringW(string.Format("CreateStarPlanets---5.2---star name[{0}],planet count[{1}]", star.name, star.planetCount));
            }
            //OutputDebugStringW(string.Format("CreateStarPlanets---10000--finished---star name[{0}],planet count[{1}]", star.name, star.planetCount));
        } //end of function
    }
}
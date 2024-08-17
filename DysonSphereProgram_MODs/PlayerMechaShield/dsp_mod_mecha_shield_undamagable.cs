using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
//Mecha:EnergyShieldResist:INT&,INT&的含义为：类名称Mecha，方法名称EnergyShieldResist，重载函数的参数签名字符串：INT&,INT&
//下面几个 //MOD_XXXX==> 是固定写法，用于给CE提供相关加载信息，必须按此格式写。
//MOD_PATCH_TARGET==>Mecha:EnergyShieldResist:INT&|Mecha:EnergyShieldResist:INT&,INT&
//MOD_NEW_METHOD==>DSP_CE_MOD.Mecha_Shield_Undamage:new_EnergyShieldResist|DSP_CE_MOD.Mecha_Shield_Undamage:new_EnergyShieldResist2
//MOD_OLD_CALLER==>DSP_CE_MOD.Mecha_Shield_Undamage:old_EnergyShieldResist|DSP_CE_MOD.Mecha_Shield_Undamage:old_EnergyShieldResist2
//MOD_DESCRIPTION==>机甲的护盾不会受到伤害
namespace DSP_CE_MOD
{
    public class Mecha_Shield_Undamage : Mecha
    {
        // 负责蓝图复制的，有专门的另外一个类：BuildTool_BlueprintCopy
        // Token: 0x06000EFA RID: 3834 RVA: 0x00108FE8 File Offset: 0x001071E8
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool new_EnergyShieldResist(ref int damage)
        {
            damage = 0;
            return true;
            /*
            //以下是原始代码
            bool result = false;
            lock (this)
            {
                if (this.player.invincible)
                {
                    damage = 0;
                }
                if (this.energyShieldEnergy > 0L)
                {
                    long num = this.energyShieldEnergy / this.energyShieldEnergyRate;
                    num = ((num < (long)damage) ? num : ((long)damage));
                    this.energyShieldEnergy -= num * this.energyShieldEnergyRate;
                    this.MarkShieldResist(damage);
                    damage -= (int)num;
                    if (this.energyShieldEnergy < this.energyShieldEnergyRate)
                    {
                        this.energyShieldEnergy = 0L;
                        if (this.energyShieldRecoverCD < 150)
                        {
                            this.energyShieldRecoverCD = 150;
                        }
                    }
                }
                result = (this.energyShieldEnergy > 0L);
            }
            return result;
            */
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool old_EnergyShieldResist(ref int damage)
        {
            // 这里是一个跳板函数。在MOD被动态载入到目标进程之后，它指向Patch之前的原始函数。
            // 当需要在新函数中，对原来函数进行调用时，执行这个函数即可

            // 本函数内部必须留空。
            // 函数名称可以自由定义，但是函数返回值类型与参数列表必须与原函数完全一致
            return true;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool new_EnergyShieldResist2(ref int damage, out int resist)
        {
            damage = 0;
            resist = 0;
            return true;
            /*
            //以下是原始代码。这个重载过的函数目前在游戏中似乎没有被引用到
            bool result = false;
            resist = 0;
            lock (this)
            {
                if (this.energyShieldEnergy > 0L && !this.player.invincible)
                {
                    long num = this.energyShieldEnergy / this.energyShieldEnergyRate;
                    resist = (int)((num < (long)damage) ? num : ((long)damage));
                    this.energyShieldEnergy -= (long)resist * this.energyShieldEnergyRate;
                    this.MarkShieldResist(damage);
                    damage -= resist;
                    if (this.energyShieldEnergy < this.energyShieldEnergyRate)
                    {
                        this.energyShieldEnergy = 0L;
                        if (this.energyShieldRecoverCD < 150)
                        {
                            this.energyShieldRecoverCD = 150;
                        }
                    }
                }
                result = (this.energyShieldEnergy > 0L);
            }
            return result;
            */
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool old_EnergyShieldResist2(ref int damage, out int resist)
        {
            resist = 0;
            return true;
        }
    }
}
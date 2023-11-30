
NewtonsoftJson=nil

function split(str,sp)
    local result = {}
    local i = 0
    local j = 0
    local num = 1
    local pos = 0
    local sz = #str
    while true do
        i , j = string.find(str,sp,i+1)
        if i == nil then
            if num ~=1 then
                result[num] = string.sub(str,pos,string.len(str))
            end
            break
        end
        result[num] = string.sub(str,pos,i-1)
        pos = i+string.len(sp)
        num = num +1
        if pos>=sz then
         break
        end
    end
    if num==1 then
      result[num]=str
    end
    return result
end

function my_mono_findMethods(namespace,classname,methodname,signature)
 if not namespace then namespace='' end
 if not signature then signature='' end

 local class=mono_findClass(namespace,classname)

 local k
 local sig
 local first_letter=string.sub(signature,1,1)
 methods = mono_class_enumMethods(class)--,false)
 for k,v in ipairs(methods) do
 --print(k,'====>',v.name)
  if v.name==methodname then
    sig=string.upper(mono_method_getSignature(v.method))
    print('*精确寻找...在',classname,'的序号',k,'处，找到了函数',v.name,'，对应的函数签名为：', sig)
    if first_letter=='!' then
      -- 第一个字符为感叹号，则说明是要求排除具有此字符串的对象
      if string.find(sig,string.upper(signature))==nil then
        return v.method
      end
    else
      if string.find(sig,string.upper(signature))~=nil then
        return v.method
      end
    end
  end
 end
 return 0
end
--print(my_mono_findMethods("Assembly-CSharp","CargoTraffic",".ctor","PlanetData"))


function write_patch_value(patch_info_obj,is_restore)
    local addr=patch_info_obj["addr"]
    local vtype=patch_info_obj["type"]
    local value=patch_info_obj["value"]
    local old_value
    if is_restore==true then
        value=patch_info_obj["old_value"]
    end
    if vtype=="byte" then
        if is_restore == false then
            patch_info_obj["value_last"]=readByte(addr)
        end
        writeByte(addr,value)
    elseif vtype=="short" then
        if is_restore == false then
            patch_info_obj["value_last"]=readSmallInteger(addr)
        end
        writeSmallInteger(addr,value)
    elseif vtype=="int" then
        if is_restore == false then
            patch_info_obj["value_last"]=readInteger(addr)
        end
        writeInteger(addr,value)
    elseif vtype=="int64" then
        if is_restore == false then
            patch_info_obj["value_last"]=readQword(addr)
        end
        writeQword(addr,value)
    elseif vtype=="float" then
        if is_restore == false then
            patch_info_obj["value_last"]=readFloat(addr)
        end
        writeFloat(addr,value)
    elseif vtype=="double" then
        if is_restore == false then
            patch_info_obj["value_last"]=readDouble(addr)
        end
        writeDouble(addr,value)
    elseif vtype=="string" then

        if is_restore == false then
            --function readString(Address, Maxlength, isWideString) : string
            patch_info_obj["value_last"]=readString(addr)
        end
        --function writeString(Address, Text, WideChar OPTIONAL) :
        writeString(addr,value)
    end
end
--[[
    local target_patch_table={
        ["PlanetFactory:ReadObjectConn:!slot"]={
            [1]={["enable"]=true,["reg"]=true,["offset"]=0x59+2,["type"]="byte", ["value"]=shift_bits, ["old_value"]=4},
            [2]={["enable"]=true,["reg"]=true,["offset"]=0x104+2,["type"]="byte", ["value"]=shift_bits, ["old_value"]=4},
        },}
    }
]]
function process_patch_work_table(work_table,is_restore)
    local str_restore
    if is_restore then
       str_restore="恢复模式"
    else
       str_restore="写入模式"
    end
    for method_name,v in pairs(work_table) do
--    print(method_name,":",v)
        local mid=0
        for k,item in ipairs(v) do
            if item["enable"]==true then
               mid=mid+1
            end
        end

        if mid>0 then

        mid=nil
        local sp=split(method_name,":")
        if #sp==3 then
           mid=my_mono_findMethods("Assembly-CSharp",sp[1],sp[2],sp[3])
        elseif #sp==2 then
           mid=mono_findMethod('Assembly-CSharp',sp[1], sp[2])
        end
        if mid~=nil -- and method_name=="PlanetFactory:WriteObjectConn"
         then
            -- compile
            local base_addr= mono_compile_method(mid)
            if base_addr~=nil and base_addr ~= 0 then
              for k,item in ipairs(v) do
                  if item["enable"]==true then
                    item["addr"]=base_addr+item["offset"]
                    if item["reg"]==true then -- 如果指定了reg为true，则将这个method_name注册为符号名
                       registerSymbol(method_name,item["addr"])
                    end
                    print(str_restore,": ",method_name," ,首地址: ",string.format("%x",base_addr)," ,目标写入地址: ",string.format("%x",item["addr"]))
                    write_patch_value(item,is_restore)
                  end
              end
            end
        end

        end -- of if mid>0 then
    end
end

gt_mod_patch_point_original_bytes=nil
if gt_mod_patch_point_original_bytes==nil then
  -- 脚本可能会被多次激活。在这里保证gt_mod_patch_point_original_bytes只创建一个实例
  gt_mod_patch_point_original_bytes={}
end

g_compiled_files_map=nil
if g_compiled_files_map==nil then
  -- 脚本可能会被多次激活。在这里保证g_compiled_files_map只创建一个实例
  g_compiled_files_map={}
end

function trim(s)
   return s:match "^%s*(.-)%s*$"
end

function retrieve_mod_info(type_str,source_string)
    local b,e=string.find(source_string,type_str..'==>')
    if b==nil or e==nil then
       print("未找到需要注入的目标函数名称，检查cs源文件中的 “"..type_str.."==>” 后面的值")
       return nil
    end
    local a=e+1
    b,e=string.find(source_string,'\n',a)
    if b==nil or e==nil then
       print("not found end of line")
      return nil
    end
    return trim(string.sub(source_string,a,b))
end

-- 用于从CE挂载的额外资源文件中，读取内容为字符串。通常用于读取C#源文件
function LoadTableFileAsString(tb_file_name)
    local fileStr = nil
    local tableFile = findTableFile(tb_file_name)
    if tableFile ~= nil then
        local stringStream = createStringStream()
        stringStream.Position = 0 -- if not set before using 'copyFrom' the 'StringStream' object will be inconsistent.
        stringStream.copyFrom(tableFile.Stream, tableFile.Stream.Size)
        fileStr = stringStream.DataString
        stringStream.destroy()
    end
    return fileStr
end
-- 用于从磁盘中加载文件
function LoadFileAsString(full_file_path)
  -- Open the file in text mode (the default mode is text)
  local file = io.open(full_file_path, "r")

  -- Check if the file was opened successfully
  if file then
      -- Read the entire file content into a string variable
      local content = file:read("*all")

      -- Close the file
      file:close()

      -- Print the file content
      --print(content)
      return content
  else
      print("Failed to open file")
  end
  return nil
  --[[
  -- DEMO of Using stream
  -- Create a new stream object
  local stream = createMemoryStream()

  -- Open the file in binary mode
  local file = io.open("C:\\Users\\tyrant\\Desktop\\Noname33u.txt", "rb")

  -- Check if the file was opened successfully
  if file then
      -- Read the entire file content
      local content = file:read("*all")

      -- Write the content to the stream
      stream.writeAnsiString(content)

      -- Close the file
      file:close()
  else
      print("Failed to open file")
  end

  -- Now 'stream' contains the file content, and you can use it as needed.
  -- Position the stream back to the beginning
  stream.Position=0

  -- Read the content from the stream
  local streamContent = stream.readAnsiString(stream.Size)


  -- Don't forget to free the stream when you're done with it
  stream.destroy()

  -- Print the stream content
  print(streamContent) ]]--
end
function Install_game_mod_with_source(csharp_source)
    local refs, sys = dotnetpatch_getAllReferences()
    local sys2=''
    if refs and sys then
       if sys==nil then
       print("sys is nil - 1")
       sys=sys2
        end
        for k,v in pairs(refs) do
            if v:find'mscorlib' then sys2 = v end
        end
    --print("6666",refs, sys)
        --sys='C:\\WINDOWS\\Microsoft.Net\\assembly\\GAC_64\\mscorlib\\v4.0_4.0.0.0__b77a5c561934e089\\mscorlib.dll'
        --sys='C:\\Program Files\\Mono\\lib\\mono\\4.5\\mscorlib.dll '
        refs[1+#refs] = sys   ---  netstandard added to refs
        --sys2="C:\\SteamLibrary\\steamapps\\common\\Dyson Sphere Program\\DSPGAME_Data\\Managed\\mscorlib.dll"
        local src_md5=stringToMD5String(csharp_source)

        local cs_dll_file=g_compiled_files_map[src_md5]
        local msg =nil
        if cs_dll_file==nil then
          print("未在缓存中找到已编译好的文件，开始编译文件内容,MD5",src_md5)
          cs_dll_file, msg =compileCS(csharp_source, refs, sys)         -- 'sys' use netstandard, failed
          if not cs_dll_file then
              print('第一次编译失败:',ansiToUtf8(tostring(msg)),'开始第二次编译==>',sys2)
              cs_dll_file, msg =compileCS(csharp_source, refs, sys2)        -- 'sys' use mscorlib, ok
          end

          --[[
          if cs_dll_file~=nil then
              local b,e=string.find(cs_dll_file,"ce%-cscode")
              if b then
                local tmpdll=cs_dll_file
                local dir_name=string.sub(cs_dll_file,1,b-1)
                --print(dir_name)
                cs_dll_file=dir_name..csharp_source_file_name..".dll"
                print("复制DLL文件：",tmpdll,"\n  到：",cs_dll_file)
                executeCodeLocalEx("DeleteFileA",cs_dll_file)
                executeCodeLocalEx("CopyFileA",tmpdll,cs_dll_file, 1)
              end
          end ]]

          g_compiled_files_map[src_md5]=cs_dll_file

        else
            print("*直接加载缓存中找到已编译好的文件：",cs_dll_file,'MD5:',src_md5)
        end

        if cs_dll_file==nil then
            if msg==nil then msg=' (?Unknown error!!!!?)' end
            --messageDialog
            print('C#源码编译失败:'..ansiToUtf8(msg)) --show compile error in a dialog instead of a lua error only
            error(ansiToUtf8(msg))
        else
            local target_method_name=nil--"DysonSphere:BeforeGameTick"
            local new_method_name=nil--"patch_space_1.patched_DysonSphere:new_BeforeGameTick"
            local old_method_caller_name=nil--"patch_space_1.patched_DysonSphere:old_BeforeGameTick"

            print("需要注入操作的DLL文件=",cs_dll_file)
            local b=retrieve_mod_info('MOD_DESCRIPTION',csharp_source)
            if b~=nil then
               if string.upper(string.sub(b,1,4))~= "UTF8" then
                b=ansiToUtf8(tostring(b))
               end
               print("本MOD的备注信息：\n",b)
            end

            target_method_name=retrieve_mod_info('MOD_PATCH_TARGET',csharp_source)
            print('MOD_PATCH_TARGET==>',target_method_name)
            new_method_name=retrieve_mod_info('MOD_NEW_METHOD',csharp_source)
            print('MOD_NEW_METHOD==>',new_method_name)
            old_method_caller_name=retrieve_mod_info('MOD_OLD_CALLER',csharp_source)
            print('MOD_OLD_CALLER==>',old_method_caller_name)
--[[
//MOD_PATCH_TARGET==>DysonSphere:BeforeGameTick
//MOD_NEW_METHOD==>patch_space_1.patched_DysonSphere:new_BeforeGameTick
//MOD_OLD_CALLER==>patch_space_1.patched_DysonSphere:old_BeforeGameTick
//MOD_DESCRIPTION==>用于演示的第一个CE的MONO MOD DLL
            local target_method_address=getAddressSafe(target_method_name) -- returns an integer
            if gt_mod_patch_point_original_bytes[target_method_name]==nil then
                -- 如果之前没有备份过最原始的字节码，则备份一次。
                local r={["addr"]=target_method_address,["bytes"]= readBytes(target_method_address, g_max_bak_bytes_count, true)}
                gt_mod_patch_point_original_bytes[target_method_name]=r
            else
                gt_mod_patch_point_original_bytes[target_method_name]["addr"]=target_method_address -- 更新一下实际的地址值
            end
]]
            if mono_loadAssemblyFromFile(cs_dll_file)==nil then
               error("Mod 注入失败。无法加载DLL文件：",cs_dll_file)
               return
            end
            print("Mod DLL文件加载成功：",cs_dll_file)
            if target_method_name=='UTIL' then
               print("此Mod为UTIL类，仅加载完成即可，不执行PATCH步骤。")
               return
            end

            -- 用英文半角逗号分隔多个需要Patch的项。下面的new_method_address和old_method_caller_address类同，并且彼此之间是一一对应的关系
            local sp_target_method_name=split(target_method_name,",")
            local sp_new_method_name=split(new_method_name,",")
            local sp_old_method_caller_name=split(old_method_caller_name,",")
            if #sp_target_method_name<=0 or #sp_new_method_name<=0 or #sp_old_method_caller_name<=0 or
              #sp_target_method_name~=#sp_new_method_name or #sp_target_method_name~=#sp_old_method_caller_name then
               print("Patch所需的 MOD_XXX 信息不完善或有误。查看前面输出的日志信息。如果是多项Patch，则各项MOD_XXX信息中的项目数量应该相等",
               #sp_target_method_name,#sp_new_method_name,#sp_old_method_caller_name)
               return
            end
            for k,tgt_name in ipairs(sp_target_method_name) do
                do_install_game_mod_work(cs_dll_file,tgt_name,sp_new_method_name[k],sp_old_method_caller_name[k])
            end
        end
    else
    if refs==nil then print("refs is nil - 2") end
       if sys==nil then
       print("sys is nil - 2")
       --sys=sys2
        end
    end
end

function Install_game_mod_from_file(csharp_source_file_name,is_utf8_file)
    --local csharp_source=LoadTableFileAsString(csharp_source_file_name) -- 使用CT文件的“内嵌文件表”来加载C#源文件
    local csharp_source=LoadFileAsString(csharp_source_file_name) -- 从某个具体的文件路径加载CS文件
    if csharp_source == nil or csharp_source=='' then
        print(csharp_source_file_name,"文件的内容为空。无法注入。")
        return
    end
    if is_utf8_file==true then
        print(csharp_source_file_name,"文件的是UTF8编码的文件，现将其转化为普通的Ansi编码。")
        csharp_source=utf8ToAnsi(csharp_source)
    --    print(csharp_source)
    end
    --print(csharp_source,'\n===========================================')
    --csharp_source=ansiToUtf8(csharp_source)
    --print(csharp_source)

    print(csharp_source_file_name,"文件内容已载入，总字节数为：",#csharp_source)
    Install_game_mod_with_source(csharp_source)
end

function do_install_game_mod_work(cs_dll_file,target_method_name,new_method_name,old_method_caller_name)

  cs_dll_file=trim(cs_dll_file)
  target_method_name=trim(target_method_name)
  new_method_name=trim(new_method_name)
  old_method_caller_name=trim(old_method_caller_name)

  print(string.format("\n对目标【%s】的 Mod 注入前的参数与诊断信息如下：\n%s",target_method_name,cs_dll_file))--,"\n",target_method_name,"\n",new_method_name,"\n",old_method_caller_name)

  local oldmethodaddress=nil
  local sp=split(target_method_name,":")
  if #sp>2 then --说明有同名函数的额外参数类型信息
    oldmethodaddress=my_mono_findMethods('',sp[1], sp[2],sp[3]) -- return the Method ID
    if oldmethodaddress~=nil and oldmethodaddress~=0 then
     oldmethodaddress=mono_compile_method(oldmethodaddress)
    else
     oldmethodaddress=nil
    end
  else
    oldmethodaddress=getAddressSafe(target_method_name)
  end

  if oldmethodaddress==nil then
     print("未找到需要Patch的目标函数地址。target_method_address=nil, name=>",target_method_name)
     return
  end
  print(target_method_name,'==>',string.format("%x",oldmethodaddress))
  local newmethodaddress=findDotNetMethodAddress(new_method_name, cs_dll_file)
  if newmethodaddress==nil then
     print("未找到需要执行的自编函数地址。new_method_address=nil, name=>",new_method_name)
     return
  end
  print(new_method_name,'==>',string.format("%x",newmethodaddress))
  local oldmethodcalleraddress=getAddressSafe(old_method_caller_name)
  if oldmethodcalleraddress==nil then
     print("未找到用于旧函数回调的函数地址。old_method_caller_address=nil, name=>",old_method_caller_name)
     return
  end
  print(old_method_caller_name,'==>',string.format("%x",oldmethodcalleraddress))

  --old code,comment at 2023-01-26
--local result, disableinfo, disablescript=InjectDotNetDetour(cs_dll_file, target_method_name,new_method_name,old_method_caller_name)
  local result, disableinfo, disablescript=detourdotnet(oldmethodaddress,newmethodaddress,oldmethodcalleraddress)

  if result==true or result==1 or result then
    print("Mod 注入操作完成...代码恢复脚本：\n", disablescript,"【Mod 注入操作成功--Success】\n")
    gt_mod_patch_point_original_bytes[target_method_name]=disablescript
  else
      print("*****Mod 注入操作失败*****")
  end
end

function Uninstall_game_mod(mod_patch_name_target_method_name)
  local scriptStr=gt_mod_patch_point_original_bytes[mod_patch_name_target_method_name]
  print('\n...开始恢复原来的代码。',mod_patch_name_target_method_name,"的代码恢复脚本==>\n",scriptStr)
  if scriptStr~=nil and #scriptStr>0 then
    --print('The scriptStr is good.')
    if autoAssemble(scriptStr) then
      print(mod_patch_name_target_method_name,'代码恢复成功。')
    else
      print(mod_patch_name_target_method_name,'代码恢复失败，脚本中可能有错误。')
    end
  end
    gt_mod_patch_point_original_bytes[mod_patch_name_target_method_name]=nil
end

function Uninstall_all_game_mod()
    for k,v in pairs(gt_mod_patch_point_original_bytes) do
        Uninstall_game_mod(k)
    end
    gt_mod_patch_point_original_bytes={}
end

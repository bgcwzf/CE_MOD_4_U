processId=nil
processImagePath=nil
function getProcessImagePath(pid)
    local open_flag=0x1F0FFF
    local hProcess=executeCodeLocalEx("OpenProcess",open_flag,0,pid)
    local m = createMemoryStream()
    m.Size = 1024
    local ret=executeCodeLocalEx("GetModuleFileNameExA",  hProcess, 0, m.Memory, 1024)
    local pa=nil
    --print(string.format("ret %d",ret))
    if ret~=nil and ret>0 then
      m.Position=0
      pa=readStringLocal(m.Memory ,  m.Size)
	-- print(FilePath)
    end
    m=nil
    return pa
end
function onOpenProcess(new_processId)
    -- print(string.format('Process opened: %d', new_processId),process)
    processId=new_processId
    processImagePath=getProcessImagePath(new_processId)
end
function print_cheat_items(addressList,indent,visited_items)
  if addressList==nil then
    return
  end
  local indentstr='|'
    local i = 0
    if indent ~=nil and math.floor(indent)>0 then
       indent=math.floor(indent)
       while i<indent do
         indentstr=indentstr..'-->'
         i=i+1
       end
    end
    if indent ==nil then indent =0 end

    local v=addressList
       print(indentstr,v.Description,'Type:',v.Type,'ID:',v.ID,'Index:',v.Index,'Count:',v.Count,'|')
       visited_items[v.ID]=v
    i=0
    --for k,v in ipairs(addressList) do
    while i<addressList.Count do
      v=addressList[i]
      print_cheat_items(v,indent+1,visited_items)
      i=i+1
    end
end
function print_all_cheat_items()
  local  addressList = getAddressList()
  local visited_items={}
  if addressList.Count >= 1 then
    local i = 0
    local v
    --for k,v in ipairs(addressList) do
    while i<addressList.Count do
      v=addressList[i]
      if visited_items[v.ID]==nil then -- 只处理那些没有访问过的记录（因为CT中的记录顺序是乱序的。子项可能在父项前面）
        if v.Count >= 1 then
           print_cheat_items(v,0,visited_items)
        else
         print('|',v.Description,'Type:',v.Type,'ID:',v.ID,'Index:',v.Index,'Count:',v.Count,'|')
         visited_items[v.ID]=v
        end
      end
      i=i+1
    end
  end
end

function find_cheat_item_by_description(desc_name)
   local addressList = getAddressList()
  if addressList.Count >= 1 then
    local i = 0

    i=0
    local v
    --for k,v in ipairs(addressList) do
    while i<addressList.Count do
      v=addressList[i]
  --[[      print(indentstr,
      -- k,v,
      v.Description,'ID:',v.ID,'Index:',v.Index,'Count:',v.Count,'|')
      ]]
      if v.Description==desc_name then
        return v
      end
      i=i+1
    end
  end
  return nil
end

function active_cheat_item_by_description_name(descpname,is_active)
    local my1 = find_cheat_item_by_description(descpname)
    if my1~= nil then
    -- print(my1.Active and 'True' or 'False')
       my1.Active=is_active -- not my1.Active
    end
end
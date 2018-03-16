function handleAjaxVPNMsg(data) {
    var firstRemoveIndexStart = data.indexOf("<SCRIPT");
    var firstRemoveIndexEnd = data.indexOf("<\/SCRIPT>");
    var newData = data;
    while (firstRemoveIndexStart != -1) {
        firstRemoveIndexStart = newData.indexOf("<SCRIPT");
        firstRemoveIndexEnd = newData.indexOf("<\/SCRIPT>");
        //alert('firstRemoveIndexStart:'+firstRemoveIndexStart);
        if (firstRemoveIndexStart != -1) {
            if (firstRemoveIndexStart == 0) {
                newData = newData.substr(firstRemoveIndexEnd + 9, newData.length);
            } else {
                newDataHead = newData.substr(0, firstRemoveIndexStart);
                newDataTail = newData.substr(firstRemoveIndexEnd + 9, newData.length);
                //alert('newDataHead'+newDataHead);
                //alert('newDataTail'+newDataTail);
                newData = newDataHead + newDataTail;
            }
        }
        //alert(newData);
    }
    //alert("before:second:"+newData);
    var secondeRemoveIndex = newData.indexOf("<\/noscript>");
    //alert(secondeRemoveIndex);
    if (secondeRemoveIndex != -1) {
        newData = newData = newData.substr(0, secondeRemoveIndex);
    }
    //alert(newData);
    return newData;
}


////使用如下方法可以將json的key值轉為小寫 
//function upperJSONKey(jsonObj) {
//    var i = 0;
//    var c_key = "";
//    for (i = 0; i < jsonObj.length; i++) {
//        for (var key in jsonObj[i]) {
//            c_key = key.toUpperCase();
//            //jsonObj[i]["\"" + key.toUpperCase() + "\""] = jsonObj[i][key];
//            if (key != c_key) {
//                //抓值到新元素
//                jsonObj[i][key.toUpperCase()] = jsonObj[i][key];
//                //刪除原本元素
//                delete (jsonObj[i][key]);
//            }
//        }
//    }
//    //for (var key in jsonObj) {
//    //    jsonObj["\"" + key.toUpperCase() + "\""] = jsonObj[key];
//    //    delete (jsonObj[key]);
//    //}
//    return jsonObj;
//}

////使用如下方法可以將json的key值轉為小寫 
//function lowerJSONKey(jsonObj) {
//    var i = 0;
//    var c_key = "";
//    for (i = 0; i < jsonObj.length; i++) {
//        for (var key in jsonObj[i]) {
//            c_key = key.toLowerCase();
//            //jsonObj[i]["\"" + key.toLowerCase() + "\""] = jsonObj[i][key];
//            if (key != c_key) {
//                //抓值到新元素
//                jsonObj[i][key.toLowerCase()] = jsonObj[i][key];
//                //刪除原本元素
//                delete (jsonObj[i][key]);
//            }
//        }
//    }
//    //for (var key in jsonObj) {
//    //    jsonObj["\"" + key.toLowerCase() + "\""] = jsonObj[key];
//    //    delete (jsonObj[key]);
//    //}
//    return jsonObj;
//}
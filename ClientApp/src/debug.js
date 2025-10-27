export function sx59(that, e) {


    //#region 全部的控制
    let b2F = ['SX05', 'SX03', 'SX04', 'SX06', 'SX26', 'SX30', 'DE01', 'IN04', 'SH01', 'IN05', 'SX10', 'SH05', 'SH02', 'DA03', 'SX17', 'SX18', 'SX19']
    let b2L = ['LBH1', 'LBL1', 'LBL2', 'LBL3', 'LBL37', 'ZZZ121', 'LBL87', 'ZZ106']
    //let b3F = ['FKey02', 'SX01', 'DA05', 'IN06', 'IN07', 'IN08', 'SX45', 'SH14', 'SX31', 'SX37', 'SX33', 'SX34', 'SX35', 'SX36', 'SX32', 'SH06', 'SH07', 'SH08', 'SH09', 'SX38','SX39', 'DE09','IN10', 'SH10','DA06', 'SX41', 'SX42', 'SX43', 'IN09', 'SX46', 'SH13', 'SH11', 'SH12']
    //let b3L = ['LBH2','ZZZ372', 'LBL95', 'ZZZ463', 'LBL374', 'ZZZ70', 'LBL190', 'ZZZ603']
    let b3F = ['FKey02', 'SX01', 'DA05', 'IN06', 'IN07', 'IN08', 'SX45', 'SH14', 'SX31', 'SX37', 'SX33', 'SX34', 'SX35', 'SX36',
        'SX32', 'SH06', 'SH07', 'SH08', 'SH09', 'SX38', 'SX39', 'DE09', 'IN10', 'SH10', 'DA06', 'SX41', 'SX42', 'SX43', 'IN09', 'SX46',
        'SH13', 'SH11', 'SH12']
    let b3L = ['LBH2', 'ZZZ372', 'LBL95', 'ZZZ463', 'LBL374', 'ZZZ70', 'LBL190', 'ZZZ603']

    let sx59 = e.value
    if (sx59 == '1') {
        //#region show b2
        for (let i = 0; i < b2F.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b2F[i])[0];
            if (b2F[i] == 'IN05' || b2F[i] == 'SX10' || b2F[i] == 'SH05' || b2F[i] == 'SH02' || b2F[i] == 'SX17') {
                obj.FShowFV = '1'
                obj.IsRequired = '0'
            } else if (b2F[i] == "SX04" || b2F[i] == "SX26" || b2F[i] == "IN04") {
                obj.FShowFV = '2'
                obj.IsRequired = '0'
            }
            else
                obj.FShowFV = obj.IsRequired = '1'
        }
        for (let i = 0; i < b2L.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b2L[i])[0];
            obj.FShowFV = '1'
        }
        //#endregion

        //#region hide b3
        for (let i = 0; i < b3F.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b3F[i])[0];
            obj.FShowFV = obj.IsRequired = '0'
        }
        for (let i = 0; i < b3L.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b3L[i])[0];
            obj.FShowFV = '0'
        }
        //#endregion

    } else if (sx59 == '2') {
        //#region show b3
        for (let i = 0; i < b3F.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b3F[i])[0];
            if (obj.FName == 'SX31' || obj.FName == 'SX37' || obj.FName == 'SX33' || obj.FName == 'SX34' || obj.FName == 'SX35'
                || obj.FName == 'SX36' || obj.FName == 'SX32' || obj.FName == 'SH06' || obj.FName == 'SH07' || obj.FName == 'SH08'
                || obj.FName == 'SH09' || obj.FName == 'SX38' || obj.FName == 'SX39' || obj.FName == 'DE09' || obj.FName == 'IN10'
                || obj.FName == 'SH10' || obj.FName == 'DA06' || obj.FName == 'SX41' || obj.FName == 'SX42' || obj.FName == 'SX43') {
                obj.FShowFV = obj.IsRequired = '0'
            } else {
                if (obj.FName == 'DA05' || obj.FName == 'IN06' || obj.FName == 'IN07') {
                    obj.FShowFV = '2'
                    obj.IsRequired = '0'
                } else if (obj.FName == 'IN08' || obj.FName == 'SX45') {
                    obj.FShowFV = obj.IsRequired = 1
                }
                else {
                    obj.FShowFV = '1'
                    obj.IsRequired = '0'
                }
            }
        }

        for (let i = 0; i < b3L.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b3L[i])[0];
            if (obj.FName == 'LBL95' || obj.FName == 'ZZZ463' || obj.FName == 'LBL374' || obj.FName == 'ZZZ70')
                obj.FShowFV = '0'
            else
                obj.FShowFV = '1'
        }
        //#endregion


        //#region hide b2
        for (let i = 0; i < b2F.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b2F[i])[0];
            obj.FShowFV = obj.IsRequired = '0'
        }
        for (let i = 0; i < b2L.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b2L[i])[0];
            obj.FShowFV = '0'
        }
        //#endregion

        //setTimeout(() => {
        //#region SX01 報銷類別 控制
        //繳款
        let apF = ['SX31', 'SX37', 'SX33', 'SX34', 'SX35', 'SX36', 'SX32', 'SH06', 'SH07', 'SH08', 'SH09']
        let apL = ['LBL95', 'ZZZ463']

        //報支
        let arF = ['SX38', 'SX39', 'DE09', 'IN10', 'SH10', 'DA06', 'SX41', 'SX42', 'SX43']
        let arL = ['LBL374', 'ZZZ70']
        var sx01 = that.oData["SX01"]
        if (sx01 == '03' || sx01 == '04') {
            //#region hide ar 報支 
            for (let i = 0; i < arF.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == arF[i])[0];
                obj.FShowFV = obj.IsRequired = '0'
            }
            for (let i = 0; i < arL.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == arL[i])[0];
                obj.FShowFV = '0'
            }
            //#endregion
            //#region hide ap繳款
            for (let i = 0; i < apF.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == apF[i])[0];
                obj.FShowFV = obj.IsRequired = '0'
            }
            for (let i = 0; i < apL.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == apL[i])[0];
                obj.FShowFV = '0'
            }
            //#endregion

        } else if (sx01 == '02') { //餘額繳回
            //#region hide ar 報支 
            for (let i = 0; i < arF.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == arF[i])[0];
                obj.FShowFV = obj.IsRequired = '0'
            }
            for (let i = 0; i < arL.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == arL[i])[0];
                obj.FShowFV = '0'
            }
            //#endregion

            //#region show ap 繳款
            let sx37 = that.oData["SX37"]
            let sx31 = that.oData["SX31"]
            for (let i = 0; i < apF.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == apF[i])[0];
                if (obj.FName == 'SX35' || obj.FName == 'SX36') {//個人field
                    if (sx37 == '1') { //個人                            
                        obj.FShowFV = obj.IsRequired = '1'
                    } else {
                        obj.FShowFV = obj.IsRequired = '0'
                    }
                }
                else if (obj.FName == 'SX33' || obj.FName == 'SX34') {//廠商field
                    if (sx37 == '1') { //個人                            
                        obj.FShowFV = obj.IsRequired = '0'
                    } else {
                        obj.FShowFV = obj.IsRequired = '1'
                    }
                } else if (obj.FName == 'SH09') {
                    switch (sx31) {
                        case '11'://現金匯款
                            obj.FShowFV = obj.IsRequired = '0'
                            break;
                        case '12'://票據
                            obj.FShowFV = obj.IsRequired = '1'
                            break;
                        case '13'://現金
                            obj.FShowFV = obj.IsRequired = '0'
                            break;
                    }
                } else if (obj.FName == 'SH08') {
                    switch (sx31) {
                        case '11'://現金匯款
                            obj.FShowFV = obj.IsRequired = '1'
                            break;
                        case '12'://票據
                            obj.FShowFV = obj.IsRequired = '0'
                            break;
                        case '13'://現金
                            obj.FShowFV = obj.IsRequired = '0'
                            break;
                    }
                } else if (obj.FName == 'SH07') {
                    switch (sx31) {
                        case '11'://現金匯款
                            obj.FShowFV = obj.IsRequired = '0'
                            break;
                        case '12'://票據
                            obj.FShowFV = obj.IsRequired = '0'
                            break;
                        case '13'://現金
                            obj.FShowFV = obj.IsRequired = '1'
                            break;
                    }
                } else {
                    obj.FShowFV = obj.IsRequired = '1'
                }
            }
            for (let i = 0; i < apL.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == apL[i])[0];
                obj.FShowFV = '1'
            }
            //#endregion
        } else if (sx01 == '01') { //暫付不足
            //#region show ar 報支 
            for (let i = 0; i < arF.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == arF[i])[0];
                if (obj.FName == 'SX41') {
                    obj.FShowFV = '1'
                    obj.IsRequired = '0'
                }
                else
                    obj.FShowFV = obj.IsRequired = '1'
            }
            for (let i = 0; i < arL.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == arL[i])[0];
                obj.FShowFV = '1'
            }
            //#endregion
            //#region hide ap繳款
            for (let i = 0; i < apF.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == apF[i])[0];
                obj.FShowFV = obj.IsRequired = '0'
            }
            for (let i = 0; i < apL.length; i++) {
                let obj = that.oBody.filter((x) => x.FName == apL[i])[0];
                obj.FShowFV = '0'
            }
            //#endregion
        }
        //#endregion

        // }, 1);
    } else {
        //#region hide b2
        for (let i = 0; i < b2F.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b2F[i])[0];
            obj.FShowFV = obj.IsRequired = '0'
        }
        for (let i = 0; i < b2L.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b2L[i])[0];
            obj.FShowFV = '0'
        }
        //#endregion

        //#region hide b3
        for (let i = 0; i < b3F.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b3F[i])[0];
            obj.FShowFV = obj.IsRequired = '0'
        }
        for (let i = 0; i < b3L.length; i++) {
            let obj = that.oBody.filter((x) => x.FName == b3L[i])[0];
            obj.FShowFV = '0'
        }
        //#endregion
    }

    if (that.oData["SX02"] == "0") {
        var objSH04 = that.isNullOrEmpty(that.oData["SH04"]) ? [] : JSON.parse(that.oData["SH04"]);
        //sx59 = that.isNullOrEmpty(that.oData["SX59"]) ? "0" :that.oData["SX59"]
        //if (objSH04.length == 0 && sx59 != '0' )
        //console.log(objSH04.length == 0 && !that.isNullOrEmpty(that.oData["SX59"]))
        if (objSH04.length == 0 && !that.isNullOrEmpty(that.oData["SX59"])) {
            that.formView.updateData("SX59", null);
            //console.log(that.oData["SX59"])
            that.alert('請先選擇驗收項目', that.$appInfo.title)
        } else {
            var f = objSH04.filter((x) => x.PurName != that.$appInfo.userInfo.userName)
            if ((that.oData["Flag"] == 'A' || that.isNullOrEmpty(that.oData["Flag"]))
                && sx59 == '1') {
                if (f.length > 0) {
                    //&& that.$appInfo.userInfo.userGuid.toLowerCase() != that.oData["SysField04"]        
                    //that.formView.updateData("SX59", '0');
                    that.formView.updateData("SX59", null);
                    that.alert('驗收項目之驗收人必須為請購人才可併行報支', that.$appInfo.title)
                }
            }
            if ((that.oData["Flag"] == 'A' || that.isNullOrEmpty(that.oData["Flag"]))
                && sx59 == '2') {
                //&& that.$appInfo.userInfo.userGuid.toLowerCase() != that.oData["SysField04"]    
                if (f.length > 0) {
                    //that.formView.updateData("SX59", '0');
                    that.formView.updateData("SX59", null);
                    that.alert('驗收項目之驗收人必須為請購人才可併行沖銷暫付款', that.$appInfo.title)
                }
            }
        }
    }
    //#endregion



}

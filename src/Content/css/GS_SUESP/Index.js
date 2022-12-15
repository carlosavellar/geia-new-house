function validaform() {
    if (frm.Nome.value == "") {
        alert("Por favor, informe o Nome Completo!");
        frm.Nome.focus();
        return false;
    }
    if (frm.Telefone.value == "") {
        alert("Por favor, informe o Telefone!");
        frm.Telefone.focus();
        return false;
    }
    if (frm.Email.value == "") {
        alert("Por favor, informe o E-mail!");
        frm.Email.focus();
        return false;
    }
    if (!Suesp.validateFormEmail2(frm.Email.value)) {
        frm.Email.focus();
        return false;
    }
    $('#parte2').show(); $('#parte1').hide();
    return true;
}
function validaform1() {
    if (frm.r1[0].checked == false && frm.r1[1].checked == false && frm.r1[2].checked == false && frm.r1[3].checked == false && frm.r1[4].checked == false) {
        alert("Por favor, responda a primeira questão!");
        frm.r1.focus();
        return false;
    }
    if (frm.r2[0].checked == false && frm.r2[1].checked == false && frm.r2[2].checked == false) {
        alert("Por favor, responda a segunda questão!");
        frm.r2.focus();
        return false;
    }
    $('#parte3').show(); $('#parte2').hide(); $('#parte1').hide();

    return true;
}
function validaform2() {
    if (frm.r3[0].checked == false && frm.r3[1].checked == false) {
        alert("Por favor, responda a terceira questão!");
        frm.r3.focus();
        return false;
    }
    if (frm.r4[0].checked == false && frm.r4[1].checked == false) {
        alert("Por favor, responda a quarta questão!");
        frm.r4.focus();
        return false;
    }
    $('#parte4').show(); $('#parte3').hide(); $('#parte2').hide(); $('#parte1').hide();

    return true;
}
function validaform3() {
    if (frm.r5[0].checked == false && frm.r5[1].checked == false && frm.r5[2].checked == false) {
        alert("Por favor, responda a quinta questão!");
        frm.r5.focus();
        return false;
    }
    if (frm.r6[0].checked == false && frm.r6[1].checked == false && frm.r6[2].checked == false) {
        alert("Por favor, responda a sexta questão!");
        frm.r6.focus();
        return false;
    }
    $('#parte5').show(); $('#parte6').hide(); $('#parte4').hide(); $('#parte3').hide(); $('#parte2').hide(); $('#parte1').hide();

    return true;
}
function validaform4() {
    if (frm.r7[0].checked == false && frm.r7[1].checked == false && frm.r7[2].checked == false) {
        alert("Por favor, responda a setíma questão!");
        frm.r7.focus();
        return false;
    }
    if (frm.r8[0].checked == false && frm.r8[1].checked == false && frm.r8[2].checked == false) {
        alert("Por favor, responda a oitava questão!");
        frm.r8.focus();
        return false;
    }
    if (frm.r9[0].checked == false && frm.r9[1].checked == false && frm.r9[2].checked == false && frm.r9[3].checked == false) {
        alert("Por favor, responda a nona questão!");
        frm.r9.focus();
        return false;
    }
    $('#parte6').show(); $('#parte5').hide(); $('#parte4').hide(); $('#parte3').hide(); $('#parte2').hide(); $('#parte1').hide();
    return true;
}
function validaform5() {
    if (frm.r10[0].checked == false && frm.r10[1].checked == false && frm.r10[2].checked == false) {
        alert("Por favor, responda a décima questão!");
        frm.r10.focus();
        return false;
    }
    var p = document.getElementById("progresso"),
    vp = 0;
    var refreshId = setInterval(function () {
        console.log(vp);
        vp += 2.5;
        p.style.width = vp + "%";
    }, 50);

    return true;

}
jQuery(document).ready(function ($) {
    $("#Telefone").mask("(99) 99999-9999");
});
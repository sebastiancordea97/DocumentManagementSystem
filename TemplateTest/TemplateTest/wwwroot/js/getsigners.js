
function sendSigners() {
    var email = $('#Email').val();
  
    console.log({ email, documentId });
    $.ajax({
        type: 'POST',
        contentType: "application/json",  
        dataType: 'json',
        url: '/User/StageDocument',
        data: JSON.stringify({
            signers: [email],
            documentId: documentId
        }),
        success: function (response) {
            window.location.href = `MessageIndex?message=${response.message}&isError=${response.isError}`;             
        },
      
    });
}

var documentId = 0;

function setDocumentId(id) {

    documentId = id;

}

$(document).on("click", "#deleteOpenModal3", function () {
    var id = $(this).data('id');
    var value = $(this).data('value');
    $("#documentNameDelete").text(value);
    $("#documentIdDelete").attr("href", "/User/DeleteDocument?id=" + id);
});

/*
$(document).on("click", "#approveButton", function () {
    var id = $(this).data('id');
   
  
    $("#approveButton").attr("href", "/User/ApproveDocument?id=" + id);
});

$(document).on("click", "#revokeButton", function () {
    var id = $(this).data('id');


    $("#revokeButton").attr("href", "/User/RevokeDocument?id=" + id);
});*/
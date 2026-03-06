$(function() {
  $(".preloader").fadeOut();
});
$(function() {
  $('[data-bs-toggle="tooltip"]').tooltip()
});
// ============================================================== 
// Login and Recover Password 
// ============================================================== 
$('#to-recover').on("click", function() {
  $("#loginform").slideUp();
  $("#recoverform").fadeIn();
});
$('#to-login').on("click", function() {
  $("#recoverform").slideUp(); 
  $("#loginform").fadeIn();
});
document.addEventListener('DOMContentLoaded', function () {
  // Ocultar preloader
  const preloader = document.querySelector('.preloader');
  if (preloader) {
    preloader.style.transition = 'opacity .5s ease';
    preloader.style.opacity = '0';
    setTimeout(function () { preloader.style.display = 'none'; }, 500);
  }

  // Bootstrap tooltips (sin jQuery)
  document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
    globalThis.bootstrap?.Tooltip && new globalThis.bootstrap.Tooltip(el);
  });

  // Login ↔ Recover Password toggle
  const loginform   = document.getElementById('loginform');
  const recoverform = document.getElementById('recoverform');
  const toRecover   = document.getElementById('to-recover');
  const toLogin     = document.getElementById('to-login');

  toRecover?.addEventListener('click', function () {
    if (loginform)   loginform.style.display   = 'none';
    if (recoverform) recoverform.style.display = 'block';
  });

  toLogin?.addEventListener('click', function () {
    if (recoverform) recoverform.style.display = 'none';
    if (loginform)   loginform.style.display   = 'block';
  });
});

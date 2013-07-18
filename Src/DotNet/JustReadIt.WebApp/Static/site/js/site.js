function initFancyBox() {
  // TODO IMM HI: remove?
  $('.fancybox-link').fancybox(
    {
      openEffect: 'elastic',
      autoSize: false,
      width: 800,
      height: 600
    });
}

$(document).ready(function () {
  initFancyBox();
});

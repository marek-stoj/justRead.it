var jri = jri || {};

jri.util = {};

jri.util.getSelectedText = function() {
  var t = '';

  if (window.getSelection) {
    t = window.getSelection();
  }
  else if (document.getSelection) {
    t = document.getSelection();
  }
  else if (document.selection) {
    t = document.selection.createRange().text;
  }

  if (typeof(t) === 'undefined' || t === null || t === undefined) {
    return '';
  }

  t = t.toString();

  if (t === null || t === undefined || t.length == 0) {
    return '';
  }

  return t;
};

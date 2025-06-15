window.animateBalance = (elId, prefix, endValue, decimals = 0, duration = 1000) => {
  const el = document.getElementById(elId);
  if (!el) return;
  let startTime = null;
  function step(ts) {
    if (!startTime) startTime = ts;
    const t = Math.min((ts - startTime) / duration, 1);
    const current = Math.round(endValue * t);
    el.textContent = 
      prefix + current.toLocaleString(undefined, { minimumFractionDigits: 0 });
    if (t < 1) 
      requestAnimationFrame(step);
  }
  requestAnimationFrame(step);
};
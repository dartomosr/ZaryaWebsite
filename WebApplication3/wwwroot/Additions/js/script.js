const dotsData = [
  { top: "63%", left: "16%", width: "3%", text: "Миссия: Заброшенный склад" },
  { top: "50%", left: "22%", width: "3.4%", text: "Город: Алеф" },
  { top: "73%", left: "25%", width: "3.2%", text: "Миссия: Штурм штаба" },
];

(function initTopLineAutoHide() {
  const topLine = document.querySelector('.topLineAndText');
  if (!topLine) return;

  let lastScrollY = window.scrollY;
  let ticking = false;

  function updateTopLine() {
    const currentScrollY = window.scrollY;
    const isScrollingUp = currentScrollY < lastScrollY;
    const isNearTop = currentScrollY <= 8;

    topLine.classList.toggle('is-hidden', !isNearTop && !isScrollingUp);
    lastScrollY = currentScrollY;
    ticking = false;
  }

  window.addEventListener('scroll', () => {
    if (ticking) return;
    window.requestAnimationFrame(updateTopLine);
    ticking = true;
  }, { passive: true });
})();

(function initAdminNavigation() {
  const nav = document.querySelector('.topNav');
  if (!nav) return;

  fetch('/api/current-user')
    .then((response) => response.ok ? response.json() : null)
    .then((user) => {
      if (!user?.isAdmin) return;

      const homeLink = nav.querySelector('.topNavLinkActive');
      if (homeLink) {
        homeLink.href = '/admin/index.html';
      }

      if (!nav.querySelector('a[href="/admin/add-news"]')) {

        const link = document.createElement('a');
        link.href = '/admin/add-news';
        link.className = 'topNavLink';
        link.textContent = 'Добавить новость в ленту';
        nav.appendChild(link);
      }

      if (nav.querySelector('form[action="/Admin/Logout"]')) return;

      const logoutForm = document.createElement('form');
      logoutForm.method = 'post';
      logoutForm.action = '/Admin/Logout';
      logoutForm.className = 'adminLogoutForm';

      const logoutButton = document.createElement('button');
      logoutButton.type = 'submit';
      logoutButton.className = 'topNavLink adminLogoutButton';
      logoutButton.setAttribute('aria-label', '\u0412\u044b\u0439\u0442\u0438');
      logoutButton.title = '\u0412\u044b\u0439\u0442\u0438';
      logoutButton.innerHTML = `
        <svg class="adminLogoutIcon" viewBox="0 0 1024 1024" aria-hidden="true" focusable="false">
          <path d="M88 112c0-35.3 28.7-64 64-64h512c35.3 0 64 28.7 64 64v164H620V176H316l214 110c37.8 19.4 61.5 58.3 61.5 100.8V848H664V728h64v184c0 35.3-28.7 64-64 64H457.2c-10.2 0-20.2-2.4-29.2-7.1L121 811.2C100.8 800.8 88 780 88 757.3V112Z"/>
          <path d="M772.7 209.4c25-25 65.5-25 90.5 0l132.1 132.1c31.3 31.3 31.3 81.9 0 113.2L863.2 586.8c-25 25-65.5 25-90.5 0s-25-65.5 0-90.5L828.9 440H572c-35.3 0-64-28.7-64-64s28.7-64 64-64h256.9l-56.2-56.1c-25-25-25-65.5 0-90.5Z"/>
        </svg>`;

      logoutForm.appendChild(logoutButton);
      nav.appendChild(logoutForm);
    })
    .catch(() => {});
})();

function handleDotClick(index, btn, completeLayer) {
  completeLayer.style.opacity = "1";
  btn.dataset.completed = "true";
}

const container = document.getElementById("dotsContainer");

if (container) {
  dotsData.forEach((dot, index) => {
    const btn = document.createElement("button");
    btn.className = "dotBtn";
    btn.style.top = dot.top;
    btn.style.left = dot.left;
    btn.style.width = dot.width;

    const baseLayer = document.createElement("div");
    baseLayer.className = "dotLayer dotBase";

    const hoverLayer = document.createElement("div");
    hoverLayer.className = "dotLayer dotHover";

    const completeLayer = document.createElement("div");
    completeLayer.className = "dotLayer dotClicked";

    const tooltip = document.createElement("div");
    tooltip.className = "tooltip";
    tooltip.textContent = dot.text;

    btn.addEventListener("mouseenter", () => {
      if (btn.dataset.completed !== "true") {
        hoverLayer.style.opacity = "1";
      }
    });

    btn.addEventListener("mouseleave", () => {
      hoverLayer.style.opacity = "0";
    });

    btn.addEventListener("click", () => {
      handleDotClick(index, btn, completeLayer);
    });

    btn.appendChild(baseLayer);
    btn.appendChild(hoverLayer);
    btn.appendChild(completeLayer);
    btn.appendChild(tooltip);

    container.appendChild(btn);
  });
}

(function fillDownBlockPostBackground() {
  const wrapper = document.querySelector('.downBlock .post-bg-wrapper');
  if (!wrapper) return;

  const images = [
    '/Additions/Media/Posts/post1.webp',
    '/Additions/Media/Posts/post2.webp',
    '/Additions/Media/Posts/post3.webp',
    '/Additions/Media/Posts/post4.webp',
    '/Additions/Media/Posts/post5.webp',
    '/Additions/Media/Posts/post6.webp',
    '/Additions/Media/Posts/post7.webp',
    '/Additions/Media/Posts/post8.webp',
    '/Additions/Media/Posts/post9.webp',
    '/Additions/Media/Posts/post10.webp'
  ];

  const count = Math.min(5, images.length);

  const shuffledImages = images
    .map((img) => ({ img, sort: Math.random() }))
    .sort((a, b) => a.sort - b.sort)
    .map((item) => item.img);

  const selectedImages = shuffledImages.slice(0, count);

  const positionSlots = [
    { left: '4%', top: '10%', width: '15%' },
    { left: '24%', top: '12%', width: '13%' },
    { left: '45%', top: '14%', width: '14%' },
    { left: '66%', top: '11%', width: '12%' },
    { left: '10%', top: '40%', width: '14%' },
    { left: '30%', top: '44%', width: '13%' },
    { left: '52%', top: '46%', width: '14%' },
    { left: '70%', top: '42%', width: '13%' },
    { left: '15%', top: '66%', width: '13%' },
    { left: '44%', top: '69%', width: '14%' }
  ];

  const shuffledSlots = positionSlots
    .map((slot) => ({ slot, sort: Math.random() }))
    .sort((a, b) => a.sort - b.sort)
    .map((item) => item.slot);

  const chosenSlots = shuffledSlots.slice(0, count);

  function randInRange(min, max) {
    return Math.random() * (max - min) + min;
  }

  function randAngle(max) {
    return `${randInRange(-max, max).toFixed(1)}deg`;
  }

  for (let i = 0; i < count; i += 1) {
    const img = document.createElement('img');
    img.className = 'post-bg';
    img.src = selectedImages[i];
    img.alt = 'Пост';
    img.style.left = chosenSlots[i].left;
    img.style.top = chosenSlots[i].top;
    img.style.width = chosenSlots[i].width;
    img.style.transform = `rotate(${randAngle(12)})`;
    wrapper.appendChild(img);
  }
})();

const btn = document.getElementById('BlackBtn');
const blackImg = document.getElementById('blackMap');

if (btn && blackImg) {
  btn.addEventListener('click', () => {
    blackImg.classList.toggle('visible');
    btn.textContent = blackImg.classList.contains('visible')
      ? 'Скрыть область ЧК'
      : 'Показать область ЧК';
  });
}

(function initScrollReveal() {
  const revealItems = document.querySelectorAll('.reveal');
  if (!revealItems.length) return;

  const observer = new IntersectionObserver((entries, obs) => {
    entries.forEach((entry) => {
      if (!entry.isIntersecting) return;
      entry.target.classList.add('is-visible');
      obs.unobserve(entry.target);
    });
  }, {
    threshold: 0.15,
    rootMargin: '0px 0px -8% 0px'
  });

  revealItems.forEach((item) => observer.observe(item));
})();

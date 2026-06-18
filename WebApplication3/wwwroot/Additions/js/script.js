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
      if (!user?.name) return;

      if (user.isAdmin) {
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
      }

      if (nav.querySelector('form[action="/User/Logout"]')) return;

      const logoutForm = document.createElement('form');
      logoutForm.method = 'post';
      logoutForm.action = '/User/Logout';
      logoutForm.className = 'adminLogoutForm';

      const logoutButton = document.createElement('button');
      logoutButton.type = 'submit';
      logoutButton.className = 'topNavLink adminLogoutButton';
      logoutButton.setAttribute('aria-label', 'Выйти');
      logoutButton.title = 'Выйти';
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

(function initMainUserPanel() {
  const loginPanel = document.querySelector('.LoginAndReg');
  if (!loginPanel) return;

  fetch('/api/current-user')
    .then((response) => response.ok ? response.json() : null)
    .then((user) => {
      if (!user?.name) return;

      const topLine = document.querySelector('.topLineAndText');
      if (topLine && !topLine.querySelector('.topUserBadge')) {
        const userBadge = document.createElement('div');
        userBadge.className = 'topUserBadge';

        const userBadgeLabel = document.createElement('span');
        userBadgeLabel.className = 'topUserBadgeLabel';
        userBadgeLabel.textContent = 'Пользователь:';

        const userBadgeName = document.createElement('span');
        userBadgeName.className = 'topUserBadgeName';
        userBadgeName.textContent = user.name;

        userBadge.appendChild(userBadgeLabel);
        userBadge.appendChild(userBadgeName);
        topLine.appendChild(userBadge);
      }

      loginPanel.classList.add('is-hidden-by-auth');

    })
    .catch(() => {});
})();

(function initAuthPanelModeSwitch() {
  const loginPanel = document.querySelector('.LoginAndReg');
  const loginForm = loginPanel?.querySelector('form');
  if (!loginPanel || !loginForm) return;
  const initialLoginForm = loginForm.cloneNode(true);

  function createTitle(text) {
    const title = document.createElement('div');
    title.className = 'mainAuthTitle';
    title.textContent = text;
    return title;
  }

  function createField(labelText, inputName, inputType, isRequired, errorClass, errorId) {
    const label = document.createElement('label');
    const input = document.createElement('input');

    label.textContent = labelText;
    input.name = inputName;
    input.type = inputType;
    input.required = isRequired;

    label.appendChild(input);

    if (errorClass && errorId) {
      const errorElement = document.createElement('div');
      errorElement.className = errorClass;
      errorElement.id = errorId;
      errorElement.setAttribute('aria-live', 'polite');
      label.appendChild(errorElement);
    }

    return label;
  }

  function showRegisterForm() {
    const registerForm = document.createElement('form');
    registerForm.method = 'post';
    registerForm.action = '/User/Register/Send';
    registerForm.noValidate = true;

    const submitButton = document.createElement('button');
    submitButton.type = 'submit';
    submitButton.textContent = 'Отправить';

    const loginButton = document.createElement('button');
    loginButton.type = 'button';
    loginButton.textContent = 'Перейти ко входу';
    loginButton.addEventListener('click', showLoginForm);

    registerForm.appendChild(createTitle('Регистрация'));
    registerForm.appendChild(createField('Логин', 'Login', 'text', true, 'loginError', 'loginError'));
    registerForm.appendChild(createField('Пароль', 'Password', 'password', true, 'PassError', 'PassError'));
    registerForm.appendChild(createField('Email (необязательно)', 'Email', 'email', false, 'loginError', 'EmailError'));
    registerForm.appendChild(submitButton);
    registerForm.appendChild(loginButton);

    loginPanel.textContent = '';
    loginPanel.appendChild(registerForm);
    applyRegisterErrors(registerForm);
  }

  function showLoginForm() {
    const restoredLoginForm = initialLoginForm.cloneNode(true);
    loginPanel.textContent = '';
    loginPanel.appendChild(restoredLoginForm);
    setupLoginForm(restoredLoginForm);
  }

  function showError(errorElement, inputElement, message) {
    if (!errorElement || !inputElement) return;

    errorElement.textContent = message;
    errorElement.classList.add('is-visible');
    inputElement.classList.add('is-invalid');
  }

  function isRegisteringMode(params) {
    return params.get('isRegisting')?.toLowerCase() === 'true';
  }

  function applyLoginErrors(activeLoginForm) {
    const params = new URLSearchParams(window.location.search);
    const error = params.get('error');
    const savedLogin = params.get('login');
    const isRegisting = isRegisteringMode(params);
    if (isRegisting || (!error && !savedLogin)) return;

    const loginInput = activeLoginForm.querySelector('input[name="Login"]');
    const passwordInput = activeLoginForm.querySelector('input[name="Password"]');
    const loginError = activeLoginForm.querySelector('#loginError');
    const passwordError = activeLoginForm.querySelector('#PassError');

    if (savedLogin && loginInput) {
      loginInput.value = savedLogin;
    }

    if (error === 'nullLogin') {
      showError(loginError, loginInput, 'Введите логин');
    } else if (error === 'nullPass') {
      showError(passwordError, passwordInput, 'Введите пароль');
    } else if (error === 'wrongLogin') {
      showError(loginError, loginInput, 'Такого логина нет');
    } else if (error === 'wrongPass') {
      showError(passwordError, passwordInput, 'Неверный пароль');
    } else if (error === 'nullFields') {
      showError(loginError, loginInput, 'Введите логин');
      showError(passwordError, passwordInput, 'Введите пароль');
    }
  }

  function applyRegisterErrors(activeRegisterForm) {
    const params = new URLSearchParams(window.location.search);
    const error = params.get('error');
    const savedLogin = params.get('login');
    const savedEmail = params.get('email');
    const isRegisting = isRegisteringMode(params);
    if (!isRegisting || (!error && !savedLogin)) return;

    const loginInput = activeRegisterForm.querySelector('input[name="Login"]');
    const passwordInput = activeRegisterForm.querySelector('input[name="Password"]');
    const emailInput = activeRegisterForm.querySelector('input[name="Email"]');
    const loginError = activeRegisterForm.querySelector('#loginError');
    const passwordError = activeRegisterForm.querySelector('#PassError');
    const emailError = activeRegisterForm.querySelector('#EmailError');

    if (savedLogin && loginInput) {
      loginInput.value = savedLogin;
    }

    if (savedEmail && emailInput) {
      emailInput.value = savedEmail;
    }

    if (error === 'nullLogin') {
      showError(loginError, loginInput, 'Введите логин');
    } else if (error === 'nullPass') {
      showError(passwordError, passwordInput, 'Введите пароль');
    } else if (error === 'wrongLogin') {
      showError(loginError, loginInput, 'Такой логин уже существует');
    } else if (error === 'wrongEmail') {
      showError(emailError, emailInput, 'Email написан неверно');
    }
  }

  function setupLoginForm(activeLoginForm) {
    if (!activeLoginForm.querySelector('.mainAuthTitle')) {
      activeLoginForm.prepend(createTitle('Вход'));
    }

    const registerButton = activeLoginForm.querySelector('button[type="button"]');
    registerButton?.addEventListener('click', showRegisterForm);
    applyLoginErrors(activeLoginForm);
  }

  const params = new URLSearchParams(window.location.search);
  const isRegisting = isRegisteringMode(params);

  if (isRegisting) {
    showRegisterForm();
    return;
  }

  setupLoginForm(loginForm);
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

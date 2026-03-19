const dotsData = [
  { top: "63%", left: "16%", width: "3%", text: "Миссия: Заброшенный склад" },
  { top: "50%", left: "22%", width: "3.4%", text: "Город: Алеф" },
  { top: "73%", left: "25%", width: "3.2%", text: "Миссия: Штурм штаба" },
];

function handleDotClick(index, btn, completeLayer) {
  completeLayer.style.opacity = "1";
  btn.dataset.completed = "true";
}

const container = document.getElementById("dotsContainer");

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

(function fillDownBlockPostBackground() {
  const wrapper = document.querySelector('.downBlock .post-bg-wrapper');
  if (!wrapper) return;

  const images = [
    'Additions/Media/Posts/post1.webp',
    'Additions/Media/Posts/post2.webp',
    'Additions/Media/Posts/post3.webp',
    'Additions/Media/Posts/post4.webp',
    'Additions/Media/Posts/post5.webp',
    'Additions/Media/Posts/post6.webp',
    'Additions/Media/Posts/post7.webp',
    'Additions/Media/Posts/post8.webp',
    'Additions/Media/Posts/post9.webp',
    'Additions/Media/Posts/post10.webp'
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

btn.addEventListener('click', () => {
  blackImg.classList.toggle('visible');
  btn.textContent = blackImg.classList.contains('visible')
    ? 'Скрыть область ЧК'
    : 'Показать область ЧК';
});
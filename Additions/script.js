const dotsData = [
  { top: "63%", left: "16%", width: "3%", text: "Миссия: Заброшенный склад" },
  { top: "50%", left: "22%", width: "3.4%", text: "Город: Алеф" },
  { top: "73%", left: "25%", width: "3.2%", text: "Миссия: Фронтир" },
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

const btn = document.getElementById('BlackBtn');
const blackImg = document.getElementById('blackMap');

btn.addEventListener('click', () => {
  blackImg.classList.toggle('visible');
  btn.textContent = blackImg.classList.contains('visible')
    ? 'Скрыть область ЧК'
    : 'Показать область ЧК';
});
(function initNews() {
  const NEWS_URL = '/Additions/json/news.json';

  async function loadNews() {
    const response = await fetch(NEWS_URL);
    if (!response.ok) {
      throw new Error('Не удалось загрузить новости');
    }
    return response.json();
  }

  function createTextElement(tagName, className, text) {
    const element = document.createElement(tagName);
    if (className) element.className = className;
    element.textContent = text;
    return element;
  }

  function renderNewsList(newsItems) {
    const grid = document.querySelector('[data-news-list]');
    if (!grid) return;

    grid.innerHTML = '';

    [...newsItems].reverse().forEach((newsItem) => {
      const card = document.createElement('a');
      card.className = 'newsCard';
      card.href = `/news-1.html?id=${encodeURIComponent(newsItem.id)}`;

      if (newsItem.image) {
        const image = document.createElement('img');
        image.className = 'news-img';
        image.src = newsItem.image;
        image.alt = newsItem.title;
        card.appendChild(image);
      }

      card.appendChild(createTextElement('div', 'newsDate', newsItem.date));
      card.appendChild(createTextElement('h2', '', newsItem.title));
      card.appendChild(createTextElement('p', '', newsItem.description));

      grid.appendChild(card);
    });
  }

  function renderNewsDetail(newsItems) {
    const detail = document.querySelector('.newsDetail');
    if (!detail) return;

    const params = new URLSearchParams(window.location.search);
    const id = params.get('id') || newsItems[0]?.id;
    const newsItem = newsItems.find((item) => item.id === id) || newsItems[0];

    if (!newsItem) {
      detail.textContent = 'Новость не найдена';
      return;
    }

    document.title = newsItem.title;

    const image = detail.querySelector('.newsDetailImage');
    if (image) {
      if (newsItem.image) {
        image.src = newsItem.image;
        image.alt = newsItem.title;
        image.hidden = false;
      } else {
        image.hidden = true;
      }
    }

    const date = detail.querySelector('.newsDetailDate');
    if (date) date.textContent = newsItem.date;

    const title = detail.querySelector('.newsDetailTitle');
    if (title) title.textContent = newsItem.title;

    const content = detail.querySelector('.newsDetailBody');
    if (content) {
      content.querySelectorAll('p').forEach((paragraph) => paragraph.remove());
      content.appendChild(createTextElement('p', '', newsItem.content));
    }
  }

  loadNews()
    .then((newsItems) => {
      renderNewsList(newsItems);
      renderNewsDetail(newsItems);
    })
    .catch((error) => {
      console.error(error);
    });
})();

import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ListsPage } from '@/pages/ListsPage';
import { ListDetailPage } from '@/pages/ListDetailPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<ListsPage />} />
        <Route path="/lists/:listId" element={<ListDetailPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

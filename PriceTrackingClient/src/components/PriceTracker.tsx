import React, { useState } from 'react';
import api from '../api';

interface SearchResult {
  productId: string;
  lowestPrice: number;
}

const PriceTracker: React.FC = () => {
  const [searchId, setSearchId] = useState('');
  const [result, setResult] = useState<SearchResult | null>(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!searchId.trim()) return;

    setLoading(true);
    setError('');
    try {
      const resp = await api.get<SearchResult>(`/ProductPrices/lowest/${searchId}`);
      setResult(resp.data);
    } catch {
      setResult(null);
      setError(`Ingen data för produkt ${searchId}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4 max-w-xl mx-auto flex flex-col items-center">
      <h1 className="text-3xl font-bold mb-6">Price Tracker</h1>

      <form onSubmit={handleSearch} className="w-full mb-6 flex gap-2 justify-center">
        <input
          type="text"
          placeholder="Skriv artikelnummer"
          value={searchId}
          onChange={e => setSearchId(e.target.value)}
          className="border p-2 flex-grow max-w-xs"
          disabled={loading}
        />
        <button
          type="submit"
          className="bg-blue-600 hover:bg-blue-700 text-white px-4 rounded disabled:opacity-50"
          disabled={loading}
        >
          {loading ? '…' : 'Sök'}
        </button>
      </form>

      {error && <p className="text-red-500 mb-4">{error}</p>}
      {result && !loading && (
        <div className="mb-6 p-4 border rounded bg-gray-800 text-white w-full text-center">
          <strong>{result.productId}</strong> — Lägsta pris (30d): {result.lowestPrice} kr
        </div>
      )}
    </div>
  );
};

export default PriceTracker;

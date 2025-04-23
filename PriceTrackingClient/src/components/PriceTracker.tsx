import React, { useState, useEffect } from 'react';
import api from '../api';

interface ProductPrice {
  id: number;
  productId: string;
  lowestPriceLast30Days: number;
  recordedAt: string;
}

const PriceTracker: React.FC = () => {
  const [prices, setPrices] = useState<ProductPrice[]>([]);
  const [searchId, setSearchId] = useState('');
  const [searchResult, setSearchResult] = useState<{ productId: string; lowestPrice: number } | null>(null);
  const [searchError, setSearchError] = useState('');
  const [loading, setLoading] = useState(false);

  const fetchAll = async () => {
    setLoading(true);
    try {
      await api.post('/ProductPrices/refresh');
      const resp = await api.get<ProductPrice[]>('/ProductPrices');
      setPrices(resp.data);
      setSearchError('');
    } catch {
      setSearchError('Kunde inte ladda om priser. Försök igen.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAll();
  }, []);

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!searchId.trim()) return;

    setLoading(true);
    try {
      const resp = await api.get<{ productId: string; lowestPrice: number }>(`/ProductPrices/lowest/${searchId}`);
      setSearchResult(resp.data);
      setSearchError('');
    } catch {
      setSearchResult(null);
      setSearchError(`Ingen data för produkt ${searchId}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4 max-w-xl mx-auto flex flex-col items-center">
      <h1 className="text-3xl font-bold mb-6">Price Tracker</h1>

      {loading && <p className="mb-4 text-gray-300">Laddar ...</p>}

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

      <button
        onClick={fetchAll}
        disabled={loading}
        className="mb-6 bg-green-600 hover:bg-green-700 text-white px-6 py-2 rounded flex items-center disabled:opacity-50"
      >
        {loading ? (
          <svg className="animate-spin h-5 w-5 mr-2 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"></path>
          </svg>
        ) : (
          <span className="mr-2">Refresh</span>
        )}
        {loading ? 'Laddar...' : 'Ladda om alla priser'}
      </button>

      {searchError && <p className="text-red-500 mb-4">{searchError}</p>}
      {searchResult && !loading && (
        <div className="mb-6 p-4 border rounded bg-gray-800 text-white w-full">
          <strong>{searchResult.productId}</strong> — Lägsta pris (30d): {searchResult.lowestPrice} kr
        </div>
      )}

      <div className="w-full overflow-x-auto">
        <table className="min-w-full bg-gray-800 text-white rounded">
          <thead>
            <tr>
              <th className="px-4 py-2 border">ID</th>
              <th className="px-4 py-2 border">Produkt</th>
              <th className="px-4 py-2 border">Lägsta pris (30d)</th>
              <th className="px-4 py-2 border">Tidpunkt</th>
            </tr>
          </thead>
          <tbody>
            {prices.map(p => (
              <tr key={p.id} className="hover:bg-gray-700">
                <td className="px-4 py-2 border text-center">{p.id}</td>
                <td className="px-4 py-2 border">{p.productId}</td>
                <td className="px-4 py-2 border text-right">{p.lowestPriceLast30Days} kr</td>
                <td className="px-4 py-2 border text-center">{new Date(p.recordedAt).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default PriceTracker;

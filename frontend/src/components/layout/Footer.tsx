import React from 'react';

export const Footer: React.FC = () => {
  return (
    <footer className="bg-white border-t border-gray-200 py-4 px-6 mt-auto">
      <div className="flex items-center justify-between">
        <p className="text-sm text-gray-600">
          © {new Date().getFullYear()} Hospital Management System. All rights reserved.
        </p>
        <div className="flex items-center gap-4">
          <a href="#" className="text-sm text-gray-600 hover:text-gray-900">
            Privacy Policy
          </a>
          <a href="#" className="text-sm text-gray-600 hover:text-gray-900">
            Terms of Service
          </a>
          <a href="#" className="text-sm text-gray-600 hover:text-gray-900">
            Support
          </a>
        </div>
      </div>
    </footer>
  );
};
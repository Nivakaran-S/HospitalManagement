'use client';


import { Card } from '../ui/card';
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
} from 'recharts';

interface ChartData {
  name: string;
  value: number;
  [key: string]: any;
}

interface LineChartComponentProps {
  title: string;
  data: ChartData[];
  dataKey: string;
  xAxisKey?: string;
}

import React from 'react';
import { PieLabelRenderProps } from 'recharts';

const renderCustomizedLabel = (props: PieLabelRenderProps) => {
  const { x, y, textAnchor, stroke, percent, name } = props;

  if (!name) return null;

  const displayPercent = (typeof percent === 'number' ? percent : 0) * 100;


  return (
    <text
      x={x}
      y={y}
      fill={stroke}
      textAnchor={textAnchor}
      dominantBaseline="central"
      style={{ fontSize: 12 }}
    >
      {`${name}: ${displayPercent.toFixed(0)}%`}
    </text>
  );
};



export const LineChartComponent: React.FC<LineChartComponentProps> = ({
  title,
  data,
  dataKey,
  xAxisKey = 'name',
}) => {
  return (
    <Card title={title}>
      <ResponsiveContainer width="100%" height={300}>
        <LineChart data={data}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey={xAxisKey} />
          <YAxis />
          <Tooltip />
          <Legend />
          <Line type="monotone" dataKey={dataKey} stroke="#3b82f6" />
        </LineChart>
      </ResponsiveContainer>
    </Card>
  );
};

interface BarChartComponentProps {
  title: string;
  data: ChartData[];
  dataKey: string;
  xAxisKey?: string;
}

export const BarChartComponent: React.FC<BarChartComponentProps> = ({
  title,
  data,
  dataKey,
  xAxisKey = 'name',
}) => {
  return (
    <Card title={title}>
      <ResponsiveContainer width="100%" height={300}>
        <BarChart data={data}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey={xAxisKey} />
          <YAxis />
          <Tooltip />
          <Legend />
          <Bar dataKey={dataKey} fill="#3b82f6" />
        </BarChart>
      </ResponsiveContainer>
    </Card>
  );
};

interface PieChartComponentProps {
  title: string;
  data: ChartData[];
}

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6'];

export const PieChartComponent: React.FC<PieChartComponentProps> = ({
  title,
  data,
}) => {
  return (
    <Card title={title}>
      <ResponsiveContainer width="100%" height={300}>
        <PieChart>
          <Pie
            data={data}
            cx="50%"
            cy="50%"
            labelLine={false}
            label={renderCustomizedLabel}
            outerRadius={80}
            fill="#8884d8"
            dataKey="value"
            >

            {data.map((entry, index) => (
              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
            ))}
          </Pie>
          <Tooltip />
        </PieChart>
      </ResponsiveContainer>
    </Card>
  );
};
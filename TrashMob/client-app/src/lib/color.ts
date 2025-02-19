const colors = ['bg-green-100', 'bg-sky-100', 'bg-rose-100', 'bg-red-100', 'bg-cyan-100', 'bg-purple-100'];

export const getIndexedColor = (index: number) => {
    return colors[index % colors.length];
};

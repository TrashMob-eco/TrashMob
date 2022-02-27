import * as React from 'react';

export const Board: React.FC = () => {
    return (
        <div className="card">
            <h2>TrashMob.eco Board of Directors</h2>
            <p>
                <ol>
                    <li><b>President:</b>  Joe Beernink</li>
                    <li><b>Vice-President:</b> Mike Rosen</li>
                    <li><b>Treasurer:</b> Terri Register</li>
                    <li><b>Secretary:</b> Sandy Lilly</li>
                    <li><b>Member:</b> Jake Dilberto</li>
                    <li><b>Member:</b> Jeremiah Steen</li>
                    <li><b>Member:</b> Tom Turchiano</li>
                    <li><b>Member:</b> Darryl Walter</li>
                </ol>
            </p>
        </div>
    );
}


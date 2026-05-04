-- Tablo daha önce varsa silip temiz bir başlangıç yapıyoruz.
DROP TABLE IF EXISTS Characters;

-- Karakterler tablomuzu oluşturuyoruz.
CREATE TABLE Characters (
    Id SERIAL PRIMARY KEY,
    CharacterType VARCHAR(50) NOT NULL, -- 'Warrior' veya 'Wizard' olacak
    Name VARCHAR(100) NOT NULL,
    Health INT NOT NULL,
    Damage INT NOT NULL,
    Mana INT NOT NULL,
    Wins INT DEFAULT 0,
    Level INT DEFAULT 1,
    XP INT DEFAULT 0,
    Gold INT DEFAULT 0
);

-- Örnek karakterlerimizi ekliyoruz.
INSERT INTO Characters (CharacterType, Name, Health, Damage, Mana) VALUES 
('Warrior', 'Arthur', 100, 15, 70),
('Warrior', 'Garen', 120, 12, 50),
('Wizard', 'Merlin', 80, 20, 80),
('Wizard', 'Ryze', 70, 25, 100),
('Warrior', 'Leonidas', 110, 18, 40);
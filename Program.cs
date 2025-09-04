using System;
using System.Collections.Generic;
using System.Threading;

public abstract class Criatura
{
    public string Nome;
    public int Vida;
    public int VidaMaxima;
    public int Ataque;
    public int Defesa;
    public bool EstaViva => Vida > 0;

    protected Criatura(string nome, int vida, int ataque, int defesa)
    {
        Nome = nome;
        Vida = vida;
        VidaMaxima = vida;
        Ataque = ataque;
        Defesa = defesa;
    }

    public abstract void Atacar(Criatura alvo);

    public virtual void ReceberDano(int dano)
    {
        if (dano > 0)
        {
            Vida -= dano;
            if (Vida < 0) Vida = 0;
            Console.WriteLine($"{Nome} recebeu {dano} de dano! Vida restante: {Vida}");
        }
    }

    public virtual void Descansar()
    {
        Vida = Math.Min(VidaMaxima, Vida + (int)(VidaMaxima * 0.3));
        Console.WriteLine($"{Nome} descansou e recuperou vida! Vida: {Vida}");
    }
}

public class Guerreiro : Criatura
{
    private Random random;
    public string Arma;
    public int Sal = 3;
    public int AguaBenta = 2;

    public Guerreiro(string nome, string arma = "Crucifixo") : base(nome, 100, 25, 15)
    {
        Arma = arma;
        random = new Random();
    }

    public override void Atacar(Criatura alvo)
    {
        Console.WriteLine($"1. Atacar com {Arma}");
        Console.WriteLine($"2. Usar Sal Grosso ({Sal} restantes)");
        Console.WriteLine($"3. Usar Água Benta ({AguaBenta} restantes)");
        Console.WriteLine($"4. Beber Água Benta para curar (+50 HP)");
        Console.WriteLine("5. Tentar fugir");
        
        Console.Write("Escolha sua ação: ");
        string escolha = Console.ReadLine();

        switch (escolha)
        {
            case "1":
                AtaqueNormal(alvo);
                break;
            case "2":
                if (Sal > 0)
                {
                    UsarSal(alvo);
                    Sal--;
                }
                else
                {
                    Console.WriteLine("Você não tem mais Sal Grosso!");
                    AtaqueNormal(alvo);
                }
                break;
            case "3":
                if (AguaBenta > 0)
                {
                    UsarAguaBenta(alvo);
                    AguaBenta--;
                }
                else
                {
                    Console.WriteLine("Você não tem mais Água Benta!");
                    AtaqueNormal(alvo);
                }
                break;
            case "4":
                if (AguaBenta > 0)
                {
                    BeberAguaBenta();
                    AguaBenta--;
                }
                else
                {
                    Console.WriteLine("Você não tem Água Benta para beber!");
                    AtaqueNormal(alvo);
                }
                break;
            case "5":
                TentarFugir(alvo);
                break;
            default:
                Console.WriteLine("Ação inválida! Atacando normalmente...");
                AtaqueNormal(alvo);
                break;
        }
    }

    private void AtaqueNormal(Criatura alvo)
    {
        if (random.NextDouble() < 0.1)
        {
            Console.WriteLine($"{Nome} errou o ataque com {Arma}!");
            return;
        }

        int danoBase = Ataque;
        bool critico = random.NextDouble() < 0.2;

        if (critico)
        {
            danoBase *= 2;
            Console.WriteLine($"{Nome} acertou um golpe crítico com {Arma}!");
        }

        int danoFinal = Math.Max(1, danoBase - (alvo.Defesa / 2));
        Console.WriteLine($"{Nome} ataca {alvo.Nome} com {Arma}!");
        alvo.ReceberDano(danoFinal);
    }

    private void UsarSal(Criatura alvo)
    {
        Console.WriteLine($"{Nome} joga Sal Grosso em {alvo.Nome}!");
        int dano = 40;
        Console.WriteLine($"O sal queima a criatura! {dano} de dano!");
        alvo.ReceberDano(dano);
    }

    private void UsarAguaBenta(Criatura alvo)
    {
        Console.WriteLine($"{Nome} asperge Água Benta em {alvo.Nome}!");
        int dano = 60;
        Console.WriteLine($"A água santa causa dano massivo! {dano} de dano!");
        alvo.ReceberDano(dano);
    }

    private void BeberAguaBenta()
    {
        int cura = 50;
        Vida = Math.Min(VidaMaxima, Vida + cura);
        Console.WriteLine($"{Nome} bebe Água Benta e recupera {cura} de vida!");
        Console.WriteLine($"Vida atual: {Vida}/{VidaMaxima}");
    }

    private void TentarFugir(Criatura alvo)
    {
        if (random.NextDouble() < 0.4)
        {
            Console.WriteLine("Você conseguiu fugir da batalha!");
            Vida = 0;
        }
        else
        {
            Console.WriteLine("Não foi possível fugir! A criatura te bloqueia!");
            alvo.Atacar(this);
        }
    }
}

public class Fantasma : Criatura
{
    private Random random;

    public Fantasma(string nome) : base(nome, 60, 18, 8)
    {
        random = new Random();
    }

    public override void Atacar(Criatura alvo)
    {
        if (random.NextDouble() < 0.25)
        {
            Console.WriteLine($"{Nome} passa através de {alvo.Nome} sem causar dano!");
            return;
        }

        int danoFinal = Math.Max(1, Ataque - (alvo.Defesa / 4));
        Console.WriteLine($"{Nome} assombra {alvo.Nome} com um toque gelado!");
        alvo.ReceberDano(danoFinal);
    }
}

public class Demonio : Criatura
{
    private Random random;

    public Demonio(string nome) : base(nome, 120, 35, 20)
    {
        random = new Random();
    }

    public override void Atacar(Criatura alvo)
    {
        if (random.NextDouble() < 0.15)
        {
            Console.WriteLine($"{Nome} invoca chamas infernais em {alvo.Nome}!");
            alvo.ReceberDano(Ataque + 10);
        }
        else
        {
            int danoFinal = Math.Max(1, Ataque - (alvo.Defesa / 2));
            Console.WriteLine($"{Nome} ataca {alvo.Nome} com garras demoníacas!");
            alvo.ReceberDano(danoFinal);
        }
    }
}

public class Ceifeiro : Demonio
{
    public Ceifeiro() : base("O Ceifeiro")
    {
        Vida = 100;
        VidaMaxima = 100;
        Ataque = 30;
        Defesa = 15;
    }

    public override void Atacar(Criatura alvo)
    {
        Console.WriteLine("A foice do Ceifeiro brilha com uma luz sinistra...");
        base.Atacar(alvo);
    }
}

public class Retorcido : Fantasma
{
    public Retorcido() : base("O Retorcido")
    {
        Vida = 130;
        VidaMaxima = 130;
        Ataque = 30;
        Defesa = 15;
    }

    public override void Atacar(Criatura alvo)
    {
        Console.WriteLine("O Retorcido emite um grito ensurdecedor...");
        base.Atacar(alvo);
    }
}

public class Batalha
{
    public bool Iniciar(Guerreiro jogador, Criatura inimigo, string recompensaTipo = "")
    {
        Console.WriteLine($"BATALHA CONTRA: {inimigo.Nome}");
        Console.WriteLine(new string('=', 30));

        int round = 1;

        while (jogador.EstaViva && inimigo.EstaViva)
        {
            Console.WriteLine($"\n--- ROUND {round} ---");
            Console.WriteLine($"{jogador.Nome}: {jogador.Vida}/100 HP");
            Console.WriteLine($"{inimigo.Nome}: {inimigo.Vida}/{inimigo.VidaMaxima} HP");
            Console.WriteLine($"Sal: {jogador.Sal} | Água Benta: {jogador.AguaBenta}");
            Console.WriteLine(new string('-', 20));

            jogador.Atacar(inimigo);

            if (!inimigo.EstaViva) break;

            Console.WriteLine($"Turno de {inimigo.Nome}!");
            inimigo.Atacar(jogador);

            round++;
            Thread.Sleep(1000);
        }

        if (jogador.EstaViva)
        {
            Console.WriteLine($"Você derrotou {inimigo.Nome}!");
            
            // Dar recompensa após derrotar o inimigo
            DarRecompensa(jogador, recompensaTipo);
            
            jogador.Descansar();
            return true;
        }
        else
        {
            Console.WriteLine($"Você foi derrotado por {inimigo.Nome}...");
            return false;
        }
    }

    private void DarRecompensa(Guerreiro jogador, string tipo)
    {
        Random random = new Random();
        
        switch (tipo)
        {
            case "fantasma":
                int salGanho = random.Next(1, 3);
                jogador.Sal += salGanho;
                Console.WriteLine($"Recompensa: +{salGanho} Sal Grosso!");
                break;
                
            case "demonio":
                int aguaGanho = random.Next(1, 2);
                jogador.AguaBenta += aguaGanho;
                Console.WriteLine($"Recompensa: +{aguaGanho} Água Benta!");
                break;
                
            case "chefe":
                jogador.Sal += 3;
                jogador.AguaBenta += 2;
                Console.WriteLine("Recompensa: +3 Sal Grosso e +2 Água Benta!");
                break;
                
            default:
                // Recompensa padrão para inimigos sem tipo específico
                if (random.NextDouble() < 0.5)
                {
                    jogador.Sal += 1;
                    Console.WriteLine("Recompensa: +1 Sal Grosso!");
                }
                else
                {
                    jogador.AguaBenta += 1;
                    Console.WriteLine(" Recompensa: +1 Água Benta!");
                }
                break;
        }
        
        Console.WriteLine($"Inventário atual: Sal({jogador.Sal}) Água({jogador.AguaBenta})");
    }
}

public class Jogo
{
    private Guerreiro jogador;
    private Batalha sistemaBatalha;

    public Jogo()
    {
        jogador = new Guerreiro("Michael Thomas", "Crucifixo Abençoado");
        sistemaBatalha = new Batalha();
    }

    public void Iniciar()
    {
        Console.WriteLine(" RUBY - A FLORESTA MALDITA ");
        Console.WriteLine("=====================================");
        Console.WriteLine("Você é Michael Thomas, em busca de sua amada.");
        Console.WriteLine("A floresta está repleta de criaturas sobrenaturais...");
        Console.WriteLine("Encontre o rubi e liberte a alma de sua esposa!");

        Thread.Sleep(2000);

        Console.WriteLine("CAPÍTULO 1: A FLORESTA NOTURNA");
        Console.WriteLine("Você adentra a floresta densa e sombria...");
        Console.WriteLine("Névoa espessa envolve tudo. Corvos observam...");
        Thread.Sleep(2000);

        if (!ExplorarFloresta()) return;

        Console.WriteLine("CAPÍTULO 2: A MANSÃO MAL-ASSOMBRADA");
        Console.WriteLine("Você encontra uma mansão abandonada...");
        Console.WriteLine("Portas rangem e vozes sussurram nas sombras...");
        Thread.Sleep(2000);

        if (!ExplorarMansao()) return;

        Console.WriteLine("CAPÍTULO 3: A CIDADE FANTASMA");
        Console.WriteLine("Através da mansão, você encontra uma cidade abandonada...");
        Console.WriteLine("Edifícios em ruínas e silêncio perturbador...");
        Thread.Sleep(2000);

        if (!ExplorarCidade()) return;

        Console.WriteLine("CAPÍTULO FINAL: O ALTAR DE SACRIFÍCIO");
        Console.WriteLine("Você encontra o altar com o rubi...");
        Console.WriteLine("A alma de sua amada está presa dentro da joia!");
        Thread.Sleep(2000);

        EnfrentarChefeFinal();
    }

    private bool ExplorarFloresta()
    {
        Console.WriteLine("Você ouve ruídos na floresta...");
        Console.WriteLine("1. Investigar os ruídos");
        Console.WriteLine("2. Continuar caminhando");
        Console.Write("Escolha: ");

        if (Console.ReadLine() == "1")
        {
            Console.WriteLine("Um Fantasma aparece!");
            Fantasma fantasma = new Fantasma("Alma Peregrina");
            if (!sistemaBatalha.Iniciar(jogador, fantasma, "fantasma")) return false;
        }

        Console.WriteLine("Você encontra um baú antigo!");
        Console.WriteLine("Dentro você encontra: +2 Sal Grosso e +1 Água Benta");
        jogador.Sal += 2;
        jogador.AguaBenta += 1;
        Console.WriteLine($"Inventário: Sal({jogador.Sal}) Água({jogador.AguaBenta})");

        Console.WriteLine("O Ceifeiro aparece na sua frente!");
        Ceifeiro ceifeiro = new Ceifeiro();
        return sistemaBatalha.Iniciar(jogador, ceifeiro, "chefe");
    }

    private bool ExplorarMansao()
    {
        Console.WriteLine("Na sala de jantar, você vê uma sombra...");
        Console.WriteLine("1. Usar Crucifixo");
        Console.WriteLine("2. Jogar Sal");
        Console.WriteLine("3. Ignorar e continuar");
        Console.Write("Escolha: ");

        string escolha = Console.ReadLine();
        if (escolha == "1")
        {
            Console.WriteLine("O crucifixo brilha, afastando a sombra!");
        }
        else if (escolha == "2" && jogador.Sal > 0)
        {
            Console.WriteLine("O sal purifica a área!");
            jogador.Sal--;
        }
        else if (escolha == "3")
        {
            Console.WriteLine("Você decide ignorar a sombra...");
        }
        else
        {
            Console.WriteLine("A sombra se transforma em um Demônio e te ataca!");
            Demonio demonio = new Demonio("Demônio da Mansão");
            if (!sistemaBatalha.Iniciar(jogador, demonio, "demonio")) return false;
        }

        Console.WriteLine("Você encontra uma Bíblia antiga!");
        Console.WriteLine("Sua fé é fortalecida! +5 de Ataque permanente");
        jogador.Ataque += 5;
        Console.WriteLine($"Ataque aumentado para: {jogador.Ataque}");

        return true;
    }

    private bool ExplorarCidade()
    {
        Console.WriteLine("Você encontra uma igreja abandonada...");
        Console.WriteLine("Dentro, encontra frascos de Água Benta!");
        jogador.AguaBenta += 3;
        Console.WriteLine($"Água Benta: {jogador.AguaBenta}");

        Console.WriteLine("Três Fantasmas aparecem!");
        for (int i = 1; i <= 3; i++)
        {
            Fantasma fantasma = new Fantasma($"Fantasma {i}");
            if (!sistemaBatalha.Iniciar(jogador, fantasma, "fantasma")) return false;
            if (i < 3)
            {
                Console.WriteLine("Próximo fantasma se aproxima...");
                Thread.Sleep(1000);
            }
        }

        return true;
    }

    private void EnfrentarChefeFinal()
    {
        Console.WriteLine("O Retorcido guarda o rubi!");
        Retorcido retorcido = new Retorcido();

        if (sistemaBatalha.Iniciar(jogador, retorcido, "chefe"))
        {
            Console.WriteLine("VOCÊ VENCEU!");
            Console.WriteLine("Michael quebra o rubi e liberta a alma de sua amada!");
            Console.WriteLine("A névoa dissipa e a paz retorna à floresta...");
            Console.WriteLine("FIM - O amor prevaleceu sobre as trevas.");
        }
        else
        {
            Console.WriteLine("GAME OVER");
            Console.WriteLine("As trevas consumiram a floresta...");
            Console.WriteLine("A alma de sua amada permanece presa para sempre...");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Jogo jogo = new Jogo();
        jogo.Iniciar();

        Console.WriteLine("\nPressione qualquer tecla para sair...");
        Console.ReadKey();
    }
}
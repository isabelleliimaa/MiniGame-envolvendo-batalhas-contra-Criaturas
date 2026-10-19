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
            if (sistemaBatalha.Iniciar(jogador, fantasma, "fantasma") == ResultadoBatalha.Derrota) return false;
        }

        Console.WriteLine("Você encontra um baú antigo!");
        Console.WriteLine("Dentro você encontra: +2 Sal Grosso e +1 Água Benta");
        jogador.Sal += 2;
        jogador.AguaBenta += 1;
        Console.WriteLine($"Inventário: Sal({jogador.Sal}) Água({jogador.AguaBenta})");

        Console.WriteLine("O Ceifeiro aparece na sua frente!");
        Ceifeiro ceifeiro = new Ceifeiro();
        return sistemaBatalha.Iniciar(jogador, ceifeiro, "chefe") != ResultadoBatalha.Derrota;
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
        else if (escolha != "1" && escolha != "2" && escolha != "3" || escolha == "2" && jogador.Sal <= 0)
        {
            Console.WriteLine("Escolha inválida ou você não tem Sal Grosso. Você segue em frente com cautela.");
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
            if (sistemaBatalha.Iniciar(jogador, fantasma, "fantasma") == ResultadoBatalha.Derrota) return false;
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

        if (sistemaBatalha.Iniciar(jogador, retorcido, "chefe") == ResultadoBatalha.Vitoria)
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
} coloca comentarios
